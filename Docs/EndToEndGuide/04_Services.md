# Service Implementation: Prompt Management

## IPromptLoader & FilePromptLoader Overview

The prompt management system provides:
- Centralized prompt storage
- Caching capabilities 
- Validation rules
- Error handling

### Interface Definition
```csharp
public interface IPromptLoader
{
    Task<string> LoadPromptAsync(string name);
    void InvalidateCache(string name = null);
    bool ValidatePrompt(string content);
}
```

### Implementation
```csharp
public class FilePromptLoader : IPromptLoader, IDisposable
{
    private readonly string _promptsPath;
    private readonly ILogger<FilePromptLoader> _logger;
    private readonly Dictionary<string, PromptCacheEntry> _cache;
    private readonly SemaphoreSlim _cacheLock;

    public FilePromptLoader(
        ILogger<FilePromptLoader> logger,
        string promptsPath = "Prompts",
        bool enableCache = true)
    {
        _logger = logger;
        _promptsPath = promptsPath;
        _cache = enableCache ? new Dictionary<string, PromptCacheEntry>() : null;
        _cacheLock = new SemaphoreSlim(1, 1);
    }

    public async Task<string> LoadPromptAsync(string name)
    {
        try
        {
            if (_cache?.TryGetValue(name, out var entry) == true)
            {
                if (!IsEntryStale(entry)) return entry.Content;
            }

            await _cacheLock.WaitAsync();
            try
            {
                var path = GetPromptPath(name);
                var content = await File.ReadAllTextAsync(path);
                
                if (!ValidatePrompt(content))
                {
                    throw new PromptValidationException(
                        $"Invalid prompt content in {name}");
                }

                UpdateCache(name, content);
                return content;
            }
            finally
            {
                _cacheLock.Release();
            }
        }
        catch (Exception ex) when (LogAndWrap(ex))
        {
            throw;
        }
    }
}
```

### Key Components

1. Cache Management
```csharp
private class PromptCacheEntry
{
    public string Content { get; set; }
    public DateTime LastUpdated { get; set; }
    public DateTime LastChecked { get; set; }
}
```

2. Validation Rules
```csharp
public bool ValidatePrompt(string content)
{
    if (string.IsNullOrWhiteSpace(content))
        return false;

    // Validate structure
    if (!content.Contains("SYSTEM:") && 
        !content.Contains("USER:"))
        return false;

    // Check size limits
    if (content.Length > MaxPromptSize)
        return false;

    return true;
}
```

Continue with OllamaChatService?