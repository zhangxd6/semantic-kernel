using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.SemanticKernel.AI.TextCompletion;
using Microsoft.SemanticKernel.Orchestration;

namespace Microsoft.SemanticKernel.Connectors.AI.Ollama.TextCompletion;
internal sealed class TextCompletionResult : ITextResult, ITextStreamingResult
{
    public TextCompletionResult(TextCompletionResponse responseData) =>
      this.ModelResult = new ModelResult(responseData);

    public ModelResult ModelResult { get; }


    public Task<string> GetCompletionAsync(CancellationToken cancellationToken = default) => Task.FromResult(this.ModelResult.GetResult<TextCompletionResponse>().Text ?? string.Empty);

    public IAsyncEnumerable<string> GetCompletionStreamingAsync(CancellationToken cancellationToken = default)
    {
        throw new System.NotImplementedException();
    }
}
