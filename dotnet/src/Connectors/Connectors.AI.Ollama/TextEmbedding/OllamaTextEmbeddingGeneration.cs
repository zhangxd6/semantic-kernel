// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.SemanticKernel.AI.Embeddings;
using Microsoft.SemanticKernel.Http;
using Microsoft.SemanticKernel.Services;

namespace Microsoft.SemanticKernel.Connectors.AI.Ollama.TextEmbedding;

public sealed class OllamaTextEmbeddingGeneration : ITextEmbeddingGeneration
{
    private readonly string _model;
    private readonly string? _endpoint;
    private readonly HttpClient _httpClient;
    private readonly Dictionary<string, string> _attributes = new();

    public OllamaTextEmbeddingGeneration(string model, HttpClient httpClient, string endpoint)
    {
        Verify.NotNullOrWhiteSpace(model);
        Verify.NotNull(httpClient);
        if (httpClient.BaseAddress == null && string.IsNullOrEmpty(endpoint))
        {
            throw new ArgumentException($"The {nameof(httpClient)}.{nameof(HttpClient.BaseAddress)} and {nameof(endpoint)} are both null or empty. Please ensure at least one is provided.");
        }

        this._model = model;
        this._httpClient = httpClient;
        this._endpoint = endpoint;
        this._attributes.Add(IAIServiceExtensions.ModelIdKey, model);
        this._attributes.Add(IAIServiceExtensions.EndpointKey, endpoint ?? httpClient.BaseAddress!.ToString());
    }

    public IReadOnlyDictionary<string, string> Attributes => this._attributes;

    public async Task<IList<ReadOnlyMemory<float>>> GenerateEmbeddingsAsync(IList<string> data, CancellationToken cancellationToken = default)
    {
        return await this.ExecuteEmbeddingRequestAsync(data, cancellationToken).ConfigureAwait(false);
    }

    private async Task<IList<ReadOnlyMemory<float>>> ExecuteEmbeddingRequestAsync(IList<string> data, CancellationToken cancellationToken)
    {
        var embeddingRequest = new TextEmbeddingRequest
        {
            Prompt = data.FirstOrDefault(),
            Model = this._model
        };

        using var httpRequestMessage = HttpRequest.CreatePostRequest(this.GetRequestUri(), embeddingRequest);

        httpRequestMessage.Headers.Add("User-Agent", HttpHeaderValues.UserAgent);

        var response = await this._httpClient.SendWithSuccessCheckAsync(httpRequestMessage, cancellationToken).ConfigureAwait(false);
        var body = await response.Content.ReadAsStringWithExceptionMappingAsync().ConfigureAwait(false);

        var embeddingResponse = JsonSerializer.Deserialize<TextEmbeddingResponse>(body);

        return new List<ReadOnlyMemory<float>> { embeddingResponse.Values };
    }

    private Uri GetRequestUri()
    {
        string? baseUrl = null;

        if (!string.IsNullOrEmpty(this._endpoint))
        {
            baseUrl = this._endpoint;
        }
        else if (this._httpClient.BaseAddress?.AbsoluteUri != null)
        {
            baseUrl = this._httpClient.BaseAddress!.AbsoluteUri;
        }
        else
        {
            throw new SKException("No endpoint or HTTP client base address has been provided");
        }

        return new Uri($"{baseUrl!.TrimEnd('/')}/api/embeddings");
    }
}
