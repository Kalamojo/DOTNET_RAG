using DocumentationSearch;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenAI;

HostApplicationBuilder builder = Host.CreateApplicationBuilder();

var openAIApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator = new OpenAIClient(openAIApiKey)
            .AsEmbeddingGenerator("text-embedding-3-small");

builder.Services.AddSingleton(embeddingGenerator);
builder.Services.AddHostedService<DocumentationSearchRunner>();

IHost host = builder.Build();
host.Run();