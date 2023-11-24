// Copyright (c) Microsoft. All rights reserved.

using System.Text.Json.Serialization;

namespace Microsoft.SemanticKernel.Connectors.AI.Ollama.TextEmbedding;

/// <summary>
/// HTTP schema to perform embedding request.
/// </summary>
public sealed class TextEmbeddingRequest
{
    [JsonPropertyName("model")]
    public string Model { get; set; }

    [JsonPropertyName("prompt")]
    public string Prompt { get; set; }

    [JsonPropertyName("options")]
    public string Options { get; set; }
}
