# Configuration Implementation

## Service Registration
```csharp
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTestGeneration(
        this IServiceCollection services, 
        Action<TestOptions> configure)
    {
        services.Configure(configure);
        services.AddTransient<IChatClient, OllamaChatService>();
        services.AddTransient<IPromptLoader, FilePromptLoader>();
        services.AddTransient<ITemplateService, HandlebarsTemplateService>();
        services.AddTransient<IChainOfThoughtPromptEngine, CSPlaywrightTestBuilderChainOfThought>();
        return services;
    }
}
```

## Template Configuration
```csharp
public class TestTemplateOptions
{
    public string TemplatesPath { get; set; } = "Templates";
    public bool UseTemplateCache { get; set; } = true;
    
    public Dictionary<string, string> DefaultValues { get; set; } = new()
    {
        { "Namespace", "PlaywrightTests" },
        { "BaseUrl", "http://localhost" }
    };
}

// Usage:
services.Configure<TestTemplateOptions>(options => {
    options.TemplatesPath = "CustomTemplates";
    options.UseTemplateCache = true;
    options.DefaultValues["BaseUrl"] = "https://staging.app.com";
});
```

## Prompt System Setup
```csharp
public class PromptOptions
{
    public string PromptsPath { get; set; } = "Prompts";
    public TimeSpan CacheDuration { get; set; } = TimeSpan.FromHours(1);
    public bool ValidatePrompts { get; set; } = true;
}

// Registration:
services.AddSingleton<IPromptLoader>(sp => {
    var logger = sp.GetRequiredService<ILogger<FilePromptLoader>>();
    var options = sp.GetRequiredService<IOptions<PromptOptions>>();
    return new FilePromptLoader(logger, options.Value);
});
```