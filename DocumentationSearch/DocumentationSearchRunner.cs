using Microsoft.Extensions.AI;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.Embeddings;

namespace DocumentationSearch;

public class DocumentationSearchRunner : IHostedService
{
    
    private readonly ITextEmbeddingGenerationService _embeddingGenerator;

    private readonly ILogger<DocumentationSearchRunner> _logger;
    private readonly IHostApplicationLifetime _appLifetime;
    
    public DocumentationSearchRunner(ITextEmbeddingGenerationService embeddingGenerator,
        ILogger<DocumentationSearchRunner> logger, IHostApplicationLifetime appLifetime)
    {
        _embeddingGenerator = embeddingGenerator;
        _logger = logger;
        _appLifetime = appLifetime;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("DocumentationSearch Runner starting.");
        _logger.LogInformation(_embeddingGenerator.ToString());

        var embedding = await _embeddingGenerator.GenerateEmbeddingAsync("monopoly");

        _logger.LogInformation(string.Join(", ", embedding.ToArray()));
        _logger.LogInformation(embedding.Length.ToString());

        _appLifetime.StopApplication();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("DocumentationSearch Runner stopping.");

        return Task.CompletedTask;
    }
}
