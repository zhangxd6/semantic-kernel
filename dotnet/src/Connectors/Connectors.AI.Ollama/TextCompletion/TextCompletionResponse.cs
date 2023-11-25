using System.Text.Json.Serialization;

namespace Microsoft.SemanticKernel.Connectors.AI.Ollama.TextCompletion;

/// <summary>
/// HTTP Schema for completion response.
/// </summary>
public sealed class TextCompletionResponse
{
    /// <summary>
    /// Completed text.
    /// </summary>
    [JsonPropertyName("response")]
    public string? Text { get; set; }
}
