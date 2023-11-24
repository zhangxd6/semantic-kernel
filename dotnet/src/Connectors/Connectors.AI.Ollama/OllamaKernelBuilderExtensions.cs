// Copyright (c) Microsoft. All rights reserved.

#pragma warning disable IDE0130
// ReSharper disable once CheckNamespace - Using NS of KernelConfig
using System.Net.Http;
using Microsoft.SemanticKernel.AI.Embeddings;
using Microsoft.SemanticKernel.Connectors.AI.Ollama.TextEmbedding;

namespace Microsoft.SemanticKernel;
#pragma warning restore IDE0130

public static class OllamaKernelBuilderExtensions
{
    public static KernelBuilder WithOllamaTextEmbeddingGenerationService(this KernelBuilder builder,
        string model,
        string endpoint,
        string? serviceId = null,
        bool setAsDefault = false,
        HttpClient? httpClient = null)
    {
        builder.WithAIService<ITextEmbeddingGeneration>(serviceId, (loggerFactory, httpHandlerFactory) => new OllamaTextEmbeddingGeneration(
            model,
            HttpClientProvider.GetHttpClient(httpHandlerFactory, httpClient: httpClient, loggerFactory),
            endpoint),
            setAsDefault);
        return builder;
    }
}
