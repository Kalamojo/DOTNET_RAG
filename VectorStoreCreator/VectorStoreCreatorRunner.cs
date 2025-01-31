using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.VectorData;

namespace VectorStoreCreator
{
    public class VectorStoreCreatorRunner : IHostedService
    {
        private readonly IVectorStoreRecordCollection<string, Doc> _vectorStore;
        private readonly ILogger<VectorStoreCreatorRunner> _logger;
        private readonly IHostApplicationLifetime _appLifetime;

        public VectorStoreCreatorRunner(IVectorStoreRecordCollection<string, Doc> vectorStore,
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

            _appLifetime.StopApplication();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("VectorStoreCreator Runner stopping.");

            return Task.CompletedTask;
        }
    }
}
