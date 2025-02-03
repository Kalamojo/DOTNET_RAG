# .NET Semantic Search

## SetUp

To run the VectorStoreCreator and DocumentationSearch projects, a few environment variables need to be provided in the project properties. These can be set in Visual Studio by right clicking on a project in the Solution Explorer and going to:

Properties &rarr Debug &rarr General &rarr Open debug launch profiles UI &rarr Environment Variables

And adding a new entry in the table. For the purpose of running these projects, please add the following environment variables to each:

1. Acquire OpenAI API key and set it as `OPENAI_API_KEY`
2. Provide an SQLite connection string for `SQLITE_CONN`
    - ex.) `Data Source=C:\Users\myUser\DOTNET_Semantic_Search\dotnet_docs.db;`

## Execution

1. Run VectorStoreCreator project and terminate it once vector store update logs stop being made
2. Run DocumentationSearch project and ask away about .NET 9
