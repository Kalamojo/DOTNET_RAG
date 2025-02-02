using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.VectorData;

namespace VectorStoreCreator
{
    public class VectorStoreCreatorRunner : IHostedService
    {
        private readonly IVectorStore _vectorStore;
        private readonly ILogger<VectorStoreCreatorRunner> _logger;
        private readonly IHostApplicationLifetime _appLifetime;

        public VectorStoreCreatorRunner(IVectorStore vectorStore,
        ILogger<VectorStoreCreatorRunner> logger, IHostApplicationLifetime appLifetime)
        {
            _vectorStore = vectorStore;
            _logger = logger;
            _appLifetime = appLifetime;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("VectorStoreCreator Runner starting.");
            _logger.LogInformation(_vectorStore.ToString());

            var collection = _vectorStore.GetCollection<ulong, Doc>(Constants.CollectionName);
            await collection.CreateCollectionIfNotExistsAsync(cancellationToken);

            _appLifetime.StopApplication();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("VectorStoreCreator Runner stopping.");

            return Task.CompletedTask;
        }
    }
}
