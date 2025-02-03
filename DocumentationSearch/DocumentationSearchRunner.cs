using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Data;
using Microsoft.SemanticKernel.Embeddings;

namespace DocumentationSearch;

public class DocumentationSearchRunner : IHostedService
{
    private readonly ITextEmbeddingGenerationService _embeddingGenerator;
    private readonly IVectorStoreRecordCollection<string, Doc> _collection;
    private readonly Kernel _kernel;
    private readonly ILogger<DocumentationSearchRunner> _logger;
    private readonly IHostApplicationLifetime _appLifetime;
    
    public DocumentationSearchRunner(ITextEmbeddingGenerationService embeddingGenerator,
        IVectorStoreRecordCollection<string, Doc> collection, Kernel kernel,
        ILogger<DocumentationSearchRunner> logger, IHostApplicationLifetime appLifetime)
    {
        _embeddingGenerator = embeddingGenerator;
        _collection = collection;
        _kernel = kernel;
        _logger = logger;
        _appLifetime = appLifetime;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("DocumentationSearch Runner starting.");
        _logger.LogInformation(_embeddingGenerator.ToString());
        _logger.LogInformation(_collection.ToString());

        var textSearch = new VectorStoreTextSearch<Doc>(_collection, _embeddingGenerator);

        var searchOptions = new TextSearchOptions() { Top = 3, Skip = 0 };
        var searchPlugin = KernelPluginFactory.CreateFromFunctions("SearchPlugin", [textSearch.CreateGetTextSearchResults(searchOptions: searchOptions)]);
        _kernel.Plugins.Add(searchPlugin);

        OpenAIPromptExecutionSettings settings = new() { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() };
        KernelArguments arguments = new(settings);

        string? input;
        while (true)
        {
            Console.WriteLine("Ask anything to LLM");
            input = Console.ReadLine();
            if (string.IsNullOrEmpty(input))
                break;
            var response = await _kernel.InvokePromptAsync(input, arguments, cancellationToken: cancellationToken);
            Console.WriteLine($"\nLLM: {response}\n");
        }

        _appLifetime.StopApplication();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("DocumentationSearch Runner stopping.");

        return Task.CompletedTask;
    }
}
