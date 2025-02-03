using DocumentationSearch;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.SemanticKernel;
using Microsoft.Data.Sqlite;

var connString = Environment.GetEnvironmentVariable("SQLITE_CONN") ?? string.Empty;
var openAIApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? string.Empty;

HostApplicationBuilder builder = Host.CreateApplicationBuilder();

builder.Services.AddOpenAIChatCompletion("gpt-4o-mini", apiKey: openAIApiKey);
builder.Services.AddKernel();
builder.Services.AddSingleton<SqliteConnection>(sp =>
{
    var connection = new SqliteConnection(connString);
    connection.LoadExtension("vec0");

    return connection;
});
builder.Services.AddSqliteVectorStoreRecordCollection<string, Doc>(Constants.CollectionName);
builder.Services.AddOpenAITextEmbeddingGeneration("text-embedding-3-small", openAIApiKey, dimensions: Constants.VectorDimensions);
builder.Services.AddHostedService<DocumentationSearchRunner>();

IHost host = builder.Build();
host.Run();