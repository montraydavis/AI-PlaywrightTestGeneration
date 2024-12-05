using System.Threading;
using System.Threading.Tasks;

namespace PlaywrightTestGenerator.PromptEngines
{
    public interface IChainOfThoughtPromptEngine
    {
        Task<TestStructure> ProcessAsync(string prompt, CancellationToken cancellationToken = default);
    }
}
