using Microsoft.Extensions.AI;
using OpenAI;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

var openAIApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

IEmbeddingGenerator<string, Embedding<float>> generator = new OpenAIClient(openAIApiKey)
    .AsEmbeddingGenerator("text-embedding-3-small");

Console.WriteLine(generator);

var embedding = await generator.GenerateEmbeddingAsync("monopoly");

Console.WriteLine(embedding);
