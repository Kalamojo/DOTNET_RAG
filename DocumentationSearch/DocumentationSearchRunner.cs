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

        var searchPlugin = textSearch.CreateWithGetTextSearchResults("SearchPlugin");
        _kernel.Plugins.Add(searchPlugin);

        // Invoke prompt and use text search plugin to provide grounding information
        OpenAIPromptExecutionSettings settings = new() { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() };
        KernelArguments arguments = new(settings);
        Console.WriteLine(await _kernel.InvokePromptAsync("Have there been any updates to escape sequences in .NET 9?", arguments));

        _appLifetime.StopApplication();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("DocumentationSearch Runner stopping.");

        return Task.CompletedTask;
    }
}
