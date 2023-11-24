// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.SemanticKernel.Text;

namespace Microsoft.SemanticKernel.Connectors.AI.Ollama.TextEmbedding;

/// <summary>
/// Represents the response from the Hugging Face text embedding API.
/// </summary>
public sealed class TextEmbeddingResponse
{
    /// <summary>
    /// The embedding vector
    /// </summary>
    [JsonPropertyName("embedding")]
    [JsonConverter(typeof(ReadOnlyMemoryConverter))]
    public ReadOnlyMemory<float> Values { get; set; }
}
