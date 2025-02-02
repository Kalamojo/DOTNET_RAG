using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.SemanticKernel;
using VectorStoreCreator;

var connString = Environment.GetEnvironmentVariable("SQLITE_CONN");
var openAIApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

HostApplicationBuilder builder = Host.CreateApplicationBuilder();

builder.Services.AddSqliteVectorStore(connString);
builder.Services.AddHostedService<VectorStoreCreatorRunner>();
builder.Services.AddOpenAITextEmbeddingGeneration("text-embedding-3-small", openAIApiKey, dimensions: Constants.VectorDimensions);

IHost host = builder.Build();
host.Run();