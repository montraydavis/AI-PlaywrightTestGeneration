using System.Threading.Tasks;

namespace PlaywrightTestGenerator.PromptLoaders
{
    public interface IPromptLoader
    {
        Task<string> LoadPromptAsync(string name);
    }
}
