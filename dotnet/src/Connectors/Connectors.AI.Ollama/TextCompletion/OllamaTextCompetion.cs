using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.SemanticKernel.AI;
using Microsoft.SemanticKernel.AI.TextCompletion;
using Microsoft.SemanticKernel.Http;
using Microsoft.SemanticKernel.Services;

namespace Microsoft.SemanticKernel.Connectors.AI.Ollama.TextCompletion;
public sealed class OllamaTextCompletion : ITextCompletion
{
    private readonly string _model;
    private readonly string? _endpoint;
    private readonly HttpClient _httpClient;
    private readonly Dictionary<string, string> _attributes = new();

    public IReadOnlyDictionary<string, string> Attributes => this._attributes;


    public OllamaTextCompletion(string model, HttpClient httpClient, string endpoint)
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

    public async Task<IReadOnlyList<ITextResult>> GetCompletionsAsync(string text, AIRequestSettings? requestSettings = null, CancellationToken cancellationToken = default)
    {
        return await this.ExecuteGetCompletionsAsync(text, cancellationToken).ConfigureAwait(false);
    }

    private async Task<IReadOnlyList<ITextResult>> ExecuteGetCompletionsAsync(string text, CancellationToken cancellationToken)
    {
        var completionRequest = new TextCompletionRequest
        {
            Model = this._model,
            Prompt = text,
            Stream = false
        };

        using var httpRequestMessage = HttpRequest.CreatePostRequest(this.GetRequestUri(), completionRequest);

        httpRequestMessage.Headers.Add("User-Agent", HttpHeaderValues.UserAgent);

        using var response = await this._httpClient.SendWithSuccessCheckAsync(httpRequestMessage, cancellationToken).ConfigureAwait(false);

        var body = await response.Content.ReadAsStringWithExceptionMappingAsync().ConfigureAwait(false);

        TextCompletionResponse? completionResponse = JsonSerializer.Deserialize<TextCompletionResponse>(body);

        if (completionResponse is null)
        {
            throw new SKException("Unexpected response from model")
            {
                Data = { { "ResponseData", body } },
            };
        }

        return new List<TextCompletionResult>() { new TextCompletionResult(completionResponse) };
    }

    public IAsyncEnumerable<ITextStreamingResult> GetStreamingCompletionsAsync(string text, AIRequestSettings? requestSettings = null, CancellationToken cancellationToken = default)
    {
        throw new System.NotImplementedException();
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

        return new Uri($"{baseUrl!.TrimEnd('/')}/api/generate");
    }
}
