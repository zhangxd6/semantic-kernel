using System.Text.Json.Serialization;

namespace Microsoft.SemanticKernel.Connectors.AI.Ollama.TextCompletion;

public sealed class TextCompletionRequest
{
    /// <summary>
    /// Prompt to complete.
    /// </summary>
    [JsonPropertyName("model")]
    public string Model { get; set; } = string.Empty;

    [JsonPropertyName("prompt")]
    public string Prompt { get; set; } = string.Empty;

    [JsonPropertyName("stream")]
    public bool Stream { get; set; } = false;

}