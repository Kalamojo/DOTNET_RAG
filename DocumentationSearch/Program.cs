using DocumentationSearch;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.SemanticKernel;
using Microsoft.Data.Sqlite;

var connString = Environment.GetEnvironmentVariable("SQLITE_CONN");
var openAIApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

var connection = new SqliteConnection(connString);
connection.LoadExtension("vec0");

IKernelBuilder kernelBuilder = Kernel.CreateBuilder();
kernelBuilder.AddOpenAIChatCompletion(modelId: "gpt-4o-mini", apiKey: openAIApiKey);
Kernel kernel = kernelBuilder.Build();

HostApplicationBuilder builder = Host.CreateApplicationBuilder();

builder.Services.AddSingleton(kernel);
builder.Services.AddSingleton(connection);
builder.Services.AddSqliteVectorStoreRecordCollection<string, Doc>(Constants.CollectionName);
builder.Services.AddOpenAITextEmbeddingGeneration("text-embedding-3-small", openAIApiKey, dimensions: Constants.VectorDimensions);
builder.Services.AddHostedService<DocumentationSearchRunner>();

IHost host = builder.Build();
host.Run();