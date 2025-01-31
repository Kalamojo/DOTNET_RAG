using DocumentationSearch;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.SemanticKernel;

HostApplicationBuilder builder = Host.CreateApplicationBuilder();

var openAIApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

builder.Services.AddOpenAITextEmbeddingGeneration("text-embedding-3-small", openAIApiKey, dimensions: Constants.VectorDimensions);
builder.Services.AddHostedService<DocumentationSearchRunner>();

IHost host = builder.Build();
host.Run();