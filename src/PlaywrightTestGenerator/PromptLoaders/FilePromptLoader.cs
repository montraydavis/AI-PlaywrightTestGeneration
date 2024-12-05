using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace PlaywrightTestGenerator.PromptLoaders
{
    public class FilePromptLoader : IPromptLoader
    {
        private readonly string _promptsPath;
        private readonly ILogger<FilePromptLoader> _logger;
        private readonly Dictionary<string, string> _cache = new();

        public FilePromptLoader(ILogger<FilePromptLoader> logger, string promptsPath = "Prompts")
        {
            _logger = logger;
            _promptsPath = promptsPath;
        }

        public async Task<string> LoadPromptAsync(string name)
        {
            if (_cache.TryGetValue(name, out var cached)) return cached;

            var path = Path.Combine(_promptsPath, $"{name}.md");
            if (!File.Exists(path))
            {
                _logger.LogError("Prompt file not found: {Path}", path);
                throw new FileNotFoundException($"Prompt file not found: {path}");
            }

            var content = await File.ReadAllTextAsync(path);
            _cache[name] = content;
            return content;
        }
    }
}
