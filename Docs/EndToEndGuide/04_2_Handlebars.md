# HandlebarsTemplateService Implementation

## Core Functionality
Converts TestStructure into code using Handlebars templates.

```csharp
public class HandlebarsTemplateService : ITemplateService, IDisposable
{
    private readonly IHandlebars _handlebars;
    private readonly TestTemplateOptions _options;
    private readonly ILogger<HandlebarsTemplateService> _logger;
    private readonly Dictionary<string, HandlebarsTemplate<object, object>> _templateCache;

    public HandlebarsTemplateService(
        IOptions<TestTemplateOptions> options,
        ILogger<HandlebarsTemplateService> logger)
    {
        _options = options.Value;
        _logger = logger;
        _templateCache = new();
        _handlebars = ConfigureHandlebars();
    }

    public async Task<string> RenderTestAsync(
        TestStructure testStructure, 
        TemplateOptions options)
    {
        try
        {
            var template = await GetTemplateAsync(options.TemplatePath);
            var data = CreateTemplateData(testStructure, options);
            return template(data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Template render failed");
            throw new TemplateRenderException("Render failed", ex);
        }
    }
}
```

## Key Components

1. Template Helpers
```csharp
private void RegisterHelpers()
{
    _handlebars.RegisterHelper("formatAction", (action) => 
        action.ToString().ToLower() switch {
            "click" => "ClickAsync",
            "fill" => "FillAsync",
            _ => $"{action}Async"
        });

    _handlebars.RegisterHelper("formatSelector", (selector) =>
        selector.ToString().StartsWith("//") ? 
            $"Locator(\"{selector}\")" : 
            $"GetByTestId(\"{selector}\")");
}
```

2. Cache Management
```csharp
private async Task<HandlebarsTemplate<object, object>> GetTemplateAsync(
    string templatePath)
{
    if (_templateCache.TryGetValue(templatePath, out var template))
        return template;

    var source = await File.ReadAllTextAsync(templatePath);
    template = _handlebars.Compile(source);
    _templateCache[templatePath] = template;
    return template;
}
```

Continue with CSPlaywrightTestBuilderChainOfThought?