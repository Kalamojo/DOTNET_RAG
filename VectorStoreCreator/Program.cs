using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.SemanticKernel;
using VectorStoreCreator;

var connString = Environment.GetEnvironmentVariable("SQLITE_CONN");

HostApplicationBuilder builder = Host.CreateApplicationBuilder();
builder.Services.AddSqliteVectorStore(connString);
builder.Services.AddHostedService<VectorStoreCreatorRunner>();

IHost host = builder.Build();
host.Run();