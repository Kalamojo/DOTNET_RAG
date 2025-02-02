using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.VectorData;
using AngleSharp;
using Microsoft.SemanticKernel.Embeddings;
using AngleSharp.Html.Dom;
using AngleSharp.Dom;

namespace VectorStoreCreator
{
    public class VectorStoreCreatorRunner : BackgroundService
    {
        private readonly IVectorStore _vectorStore;
        private readonly ITextEmbeddingGenerationService _embeddingGenerator;
        private readonly ILogger<VectorStoreCreatorRunner> _logger;

        public VectorStoreCreatorRunner(IVectorStore vectorStore, ITextEmbeddingGenerationService embeddingGenerator,
        ILogger<VectorStoreCreatorRunner> logger)
        {
            _vectorStore = vectorStore;
            _embeddingGenerator = embeddingGenerator;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("VectorStoreCreator Runner starting.");
            
            var collection = _vectorStore.GetCollection<string, Doc>(Constants.CollectionName);
            await collection.CreateCollectionIfNotExistsAsync(cancellationToken);
            
            var urls = await RetrievePageUrls("https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-9/overview", cancellationToken);
            _logger.LogInformation(string.Join('\n', urls));

            await Task.WhenAll(urls.Select(url => Task.Run(() => LoadPage(url, collection, cancellationToken))));

            _logger.LogInformation("Howdy folks");
        }

        private async Task<HashSet<string>> RetrievePageUrls(string startingUrl, CancellationToken cancellationToken)
        {
            HashSet<string> urls = new() { startingUrl };

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(startingUrl, cancellationToken);
                string htmlContent = await response.Content.ReadAsStringAsync(cancellationToken);

                var config = Configuration.Default.WithDefaultLoader();
                var context = BrowsingContext.New(config);
                var document = await context.OpenAsync(req => req.Content(htmlContent), cancellationToken);

                var linkElements = document.QuerySelectorAll("#main .content a");
                var foundUrls = linkElements?.Where(link => link.TextContent.StartsWith("What's new"))
                    .Select(link => ToAbsolute(startingUrl, link.GetAttribute("href")));
                
                foreach (var url in foundUrls)
                {
                    urls.Add(url);
                }
            }

            return urls;
        }

        private async Task LoadPage(string url, IVectorStoreRecordCollection<string, Doc> collection, CancellationToken cancellationToken)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                string htmlContent = await response.Content.ReadAsStringAsync();

                var config = Configuration.Default.WithDefaultLoader();
                var context = BrowsingContext.New(config);
                var document = await context.OpenAsync(req => req.Content(htmlContent));

                var mainPage = document.QuerySelector("#main .content");
                var pageChildren = mainPage.Children.ToList();
                string currentSection = mainPage?.QuerySelector("h1")?.TextContent;
                var sections = mainPage?.QuerySelectorAll("h2")
                    .Select(heading => heading.TextContent);
                int startInd = 0;

                IEnumerable<Task> tasks = new List<Task>();
                foreach (string nextSection in sections.Skip(1).SkipLast(1))
                {
                    var endInd = pageChildren.Skip(startInd).ToList()
                        .FindIndex(item => item is IHtmlHeadingElement && item.TextContent == nextSection);
                    var sectionElements = pageChildren.Slice(startInd, endInd + 1);
                    tasks.Append(Task.Run(() => LoadSection(currentSection, url, sectionElements, collection, cancellationToken)));
                    
                    startInd = endInd;
                    currentSection = nextSection;
                }

                await Task.WhenAll(tasks);
            }
        }

        private async Task LoadSection(string section, string url, IEnumerable<IElement> contents, IVectorStoreRecordCollection<string, Doc> collection, CancellationToken cancellationToken)
        {
            var sectionContent = string.Join("\n", contents.Select(item => item.TextContent));

            _logger.LogInformation("Starting {URL} - {Section} loaded...", url, section);

            await collection.UpsertAsync(new Doc()
            {
                DocId = Ulid.NewUlid().ToString(),
                SectionName = section,
                Text = sectionContent,
                Link = url,
                Embedding = await _embeddingGenerator.GenerateEmbeddingAsync(sectionContent, cancellationToken: cancellationToken)
            }, cancellationToken: cancellationToken);

            _logger.LogInformation("Completed {URL} - {Section} loaded", url, section);
        }

        private static string ToAbsolute(string startingUrl, string? relativeUrl)
        {
            return new Uri(new Uri(startingUrl), relativeUrl).AbsoluteUri;
        }
    }
}
