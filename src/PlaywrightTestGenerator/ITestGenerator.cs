using System.Threading;
using System.Threading.Tasks;

namespace PlaywrightTestGenerator
{
    public interface ITestGenerator
    {
        Task<string> GenerateTestAsync(string pageDescription, CancellationToken cancellationToken = default);
        Task<string> GenerateTestAsync(TestGenerationRequest request, CancellationToken cancellationToken = default);
    }
}
