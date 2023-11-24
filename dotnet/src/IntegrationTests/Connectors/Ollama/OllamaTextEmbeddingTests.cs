using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel.AI.Embeddings;
using Microsoft.SemanticKernel.Connectors.AI.Ollama.TextEmbedding;
using Microsoft.SemanticKernel.Connectors.AI.OpenAI.TextEmbedding;
using SemanticKernel.IntegrationTests.TestSettings;
using Xunit;
using Xunit.Abstractions;

namespace SemanticKernel.IntegrationTests.Connectors.Ollams;

public sealed class OllamaTextEmbeddingTests : IDisposable
{
    private RedirectOutput _testOutputHelper;
    private string endpoint = "http://host.docker.internal:11434";
    private string model = "llama2";

    public OllamaTextEmbeddingTests(ITestOutputHelper output)
    {

        this._testOutputHelper = new RedirectOutput(output);
        Console.SetOut(this._testOutputHelper);
    }


    [Fact]
    public async Task OllamaTextEmbeddingTestAsync()
    {
        // Arrange
        const string Input = "This is test";

        using var httpClient = new HttpClient();

        var testembedding = new OllamaTextEmbeddingGeneration(this.model, httpClient, this.endpoint);

        // Act
        var remoteResponse = await testembedding.GenerateEmbeddingAsync(Input);

        // Assert
        Assert.NotNull(remoteResponse);

        //Assert.StartsWith(Input, remoteResponse, StringComparison.Ordinal);
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