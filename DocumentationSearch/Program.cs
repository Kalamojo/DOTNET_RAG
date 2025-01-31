using DocumentationSearch;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.SemanticKernel;

var openAIApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

HostApplicationBuilder builder = Host.CreateApplicationBuilder();
builder.Services.AddOpenAITextEmbeddingGeneration("text-embedding-3-small", openAIApiKey, dimensions: Constants.VectorDimensions);
builder.Services.AddHostedService<DocumentationSearchRunner>();

IHost host = builder.Build();
host.Run();