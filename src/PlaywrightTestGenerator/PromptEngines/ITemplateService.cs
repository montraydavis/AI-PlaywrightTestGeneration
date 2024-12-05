using System.Threading.Tasks;

namespace PlaywrightTestGenerator.PromptEngines
{
    public interface ITemplateService
    {
        Task<string> RenderTestAsync(TestStructure testStructure, TemplateOptions options);
    }
}