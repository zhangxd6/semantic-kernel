using System.Net.Http;
using Microsoft.SemanticKernel.AI.Embeddings;
using Microsoft.SemanticKernel.Connectors.AI.Ollama.TextEmbedding;
using Microsoft.SemanticKernel.Plugins.Memory;

namespace Microsoft.SemanticKernel;
public static class OllamaMemoryBuilderExtensions
{
    public static MemoryBuilder WithOllamaTextEmbeddingGenerationService(this MemoryBuilder builder,
        string model,
        string endpoint,
        string? serviceId = null,
        bool setAsDefault = false,
        HttpClient? httpClient = null)
    {
        builder.WithTextEmbeddingGeneration<ITextEmbeddingGeneration>((loggerFactory, httpHandlerFactory) => new OllamaTextEmbeddingGeneration(
            model,
            HttpClientProvider.GetHttpClient(httpHandlerFactory, httpClient: httpClient, loggerFactory),
            endpoint)
            );
        return builder;
    }
}