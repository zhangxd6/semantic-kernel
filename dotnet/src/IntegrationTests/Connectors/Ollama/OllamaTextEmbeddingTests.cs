using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI.Embeddings;
using Microsoft.SemanticKernel.Connectors.AI.Ollama.TextCompletion;
using Xunit;
using Xunit.Abstractions;
using Microsoft.SemanticKernel.Plugins.Memory;
using System.Linq;

namespace SemanticKernel.IntegrationTests.Connectors.Ollams;

public sealed class OllamaTextCompletionTests : IDisposable
{
    private RedirectOutput _testOutputHelper;
    private string endpoint = "http://host.docker.internal:11434";
    private string model = "llama2";

    public OllamaTextCompletionTests(ITestOutputHelper output)
    {

        this._testOutputHelper = new RedirectOutput(output);
        Console.SetOut(this._testOutputHelper);
    }


    [Fact]
    public async Task OllamaTextEmbeddingTestAsync()
    {
        // Arrange
        const string Input = "What is Llama?";

        using var httpClient = new HttpClient();

        var testembedding = new OllamaTextCompletion(this.model, httpClient, this.endpoint);

        // Act
        var remoteResponse = await testembedding.GetCompletionsAsync(Input);

        // Assert
        Assert.NotNull(remoteResponse);

        //Assert.StartsWith(Input, remoteResponse, StringComparison.Ordinal);
    }

    [Fact]
    public async Task MemoryTest()
    {
        // Given
        var memoryBuilder = new MemoryBuilder();

        memoryBuilder.WithOllamaTextEmbeddingGenerationService("llama2", "http://host.docker.internal:11434");
        memoryBuilder.WithMemoryStore(new VolatileMemoryStore());

        var memory = memoryBuilder.Build();
        const string MemoryCollectionName = "aboutMe";

        await memory.SaveInformationAsync(MemoryCollectionName, id: "info1", text: "My name is Andrea");
        await memory.SaveInformationAsync(MemoryCollectionName, id: "info2", text: "I currently work as a tourist operator");
        await memory.SaveInformationAsync(MemoryCollectionName, id: "info3", text: "I currently live in Seattle and have been living there since 2005");
        await memory.SaveInformationAsync(MemoryCollectionName, id: "info4", text: "I visited France and Italy five times since 2015");
        await memory.SaveInformationAsync(MemoryCollectionName, id: "info5", text: "My family is from New York");
        // When
        var info = await memory.SearchAsync(MemoryCollectionName, "what is my name?", minRelevanceScore: 0.1).FirstOrDefaultAsync();

        // Then
        Assert.NotNull(info);
    }


    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (disposing)
        {
            this._testOutputHelper.Dispose();
        }
    }
}