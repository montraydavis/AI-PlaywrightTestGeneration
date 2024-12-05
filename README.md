# Playwright Test Generator Documentation

1. Getting Started
   - Installation
   - Configuration
   - Quick Start Guide

2. Core Components
   - Test Structure Generator
   - Template Service
   - Prompt Engine
   - Chat Service Integration

3. Prompt System
   - Element Extraction
   - Task Extraction 
   - Test Structure Generation
   - Custom Prompts
   - Best Practices

4. Test Generation
   - Test Structure Model
   - Page Elements
   - User Tasks
   - Test Cases
   - Assertions

5. Templates
   - Handlebars Integration
   - Custom Templates
   - Template Variables
   - Helper Functions

6. API Reference
   - ITestGenerator
   - ITemplateService
   - IChainOfThoughtPromptEngine
   - Builders and Models

7. Advanced Usage
   - Custom Implementations
   - Extensions
   - Error Handling
   - Performance Optimization

8. Examples
   - Basic Login Page
   - Complex Forms
   - Data-Driven Tests
   - Custom Scenarios

---

# Getting Started

## Installation

```bash
dotnet restore
```

Required dependencies:
- .NET 8.0
- Ollama
- Handlebars.NET

## Configuration

### Basic Setup
```csharp
var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddTestGeneration(options => {
    options.TemplatesPath = "Templates";
    options.UseTemplateCache = true;
    options.DefaultTemplateValues = new Dictionary<string, string> {
        { "Ns", "PlaywrightTests" },
        { "BaseUrl", "http://localhost:3000" }
    };
});
```

### Ollama Setup
1. Install Ollama
2. Run model:
```bash
ollama run llama2:latest
```

## Quick Start

1. Create page description:
```csharp
var description = @"
Login page:
- Username field (id: username)
- Password field (id: password)
- Login button (text: Sign In)
- Error message (class: error-msg)
";
```

2. Generate test:
```csharp
var generator = host.Services.GetRequiredService<ITestGenerator>();
var test = await generator.GenerateTestAsync(description);
```

3. Output: Generated Playwright test in C#/NUnit format

---

# Core Components

## Test Structure Generator
- Main orchestrator component
- Processes page descriptions through 3 phases:
  - Element extraction
  - Task identification
  - Test case generation

```csharp
public interface ITestGenerator
{
    Task<string> GenerateTestAsync(string pageDescription);
}
```

## Template Service
- Handlebars-based code generation
- Configurable templates for different test frameworks
- Built-in helpers for Playwright actions

```csharp
public interface ITemplateService
{
    Task<string> RenderTestAsync(TestStructure structure, TemplateOptions options);
}
```

## Prompt Engine
- Chain-of-thought reasoning for test generation
- Markdown-based prompt system
- Configurable via `Prompts/*.md` files

```csharp
public interface IChainOfThoughtPromptEngine
{
    Task<TestStructure> ProcessAsync(string prompt);
}
```

## Chat Service Integration
- Ollama integration for LLM processing
- Configurable model selection
- Streaming support for real-time generation

---

# Prompt System

## Prompt Structure
```
Prompts/
  ├── ElementExtraction.md   # UI element identification
  ├── TaskExtraction.md      # User workflow analysis
  └── TestStructureGeneration.md # Test case creation
```

## Element Extraction
Input: Page description
Output: JSON elements list
```json
{
    "elements": [
        {
            "name": "loginButton",
            "selector": "#login",
            "type": "button"
        }
    ]
}
```

## Task Extraction
Input: Elements + page description
Output: User workflows
```json
{
    "tasks": [
        {
            "name": "loginWithValidCredentials",
            "steps": [
                {
                    "elementName": "usernameInput",
                    "action": "fill"
                }
            ]
        }
    ]
}
```

## Test Structure Generation
Input: Elements + tasks
Output: Complete test cases with assertions

---

# Test Generation

## Test Structure Model
```csharp
public class TestStructure 
{
    public string PageName { get; set; }
    public List<PageElement> Elements { get; set; }
    public List<UserTask> Tasks { get; set; }
    public List<TestCase> TestCases { get; set; }
}
```

## Page Elements
- Name conventions: camelCase
- Selector priority: id > data-testid > aria > css > xpath
- Properties for validation and interaction

## User Tasks
```csharp
public class UserTask
{
    public string Name { get; set; }
    public List<TaskStep> Steps { get; set; }
    public List<string> Prerequisites { get; set; }
    public List<string> ExpectedResults { get; set; }
}
```

## Test Cases
- Setup/teardown
- Steps with parameters
- Assertions
- Tags for categorization

---

# Templates

## Handlebars Integration
```csharp
services.AddSingleton<ITemplateService, HandlebarsTemplateService>();
```

## Custom Templates
Template location: `Templates/CSTest.hbs`
```handlebars
namespace {{ns}} {
    [TestFixture]
    public class {{pageName}}Tests : PageTest {
        {{#each elements}}
        private ILocator {{name}} => _page.Locator("{{selector}}");
        {{/each}}
    }
}
```

## Helper Functions
```csharp
_handlebars.RegisterHelper("formatAction", (action) => 
    action switch {
        "click" => "ClickAsync",
        "fill" => "FillAsync",
        _ => $"{action}Async"
    }
);
```

## Variables
- ns: Namespace
- baseUrl: Test environment URL
- pageName: Class name
- elements: Page elements
- testCases: Generated tests

---

# API Reference

## ITestGenerator
```csharp
public interface ITestGenerator
{
    Task<string> GenerateTestAsync(string pageDescription);
    Task<string> GenerateTestAsync(TestGenerationRequest request);
}
```

## ITemplateService
```csharp
public interface ITemplateService
{
    Task<string> RenderTestAsync(
        TestStructure testStructure, 
        TemplateOptions options
    );
}
```

## TestGenerationRequest
```csharp
public class TestGenerationRequest
{
    public string PageDescription { get; set; }
    public Dictionary<string, string> AdditionalContext { get; set; }
    public TestGenerationOptions Options { get; set; }
}
```

## Builder Pattern
```csharp
var request = new TestGenerationRequestBuilder()
    .WithPageDescription(description)
    .WithContext("environment", "staging")
    .WithOptions(opt => {
        opt.WithComments()
           .WithTestFramework("NUnit");
    })
    .Build();
```

---

# Advanced Usage

## Custom Implementations
```csharp
public class CustomPromptEngine : IChainOfThoughtPromptEngine 
{
    public async Task<TestStructure> ProcessAsync(string prompt) {
        // Custom logic
    }
}
```

## Extensions
```csharp
services.AddTestGeneration(options => {
    options.UseCustomPromptLoader<MyPromptLoader>();
    options.AddCustomHelpers(helpers => {
        helpers.Add("customFormat", CustomFormatter);
    });
});
```

## Error Handling
```csharp
try {
    await generator.GenerateTestAsync(description);
} catch (TemplateNotFoundException ex) {
    logger.LogError("Template missing: {Path}", ex.Message);
} catch (TemplateRenderException ex) {
    logger.LogError("Render failed: {Error}", ex.Message);
}
```

## Performance
- Template caching
- Prompt reuse
- Parallel test generation
- Streaming responses

---

# Examples

## Login Page Example
```csharp
var description = @"
Login page:
- Username field (#username)
- Password field (#password)
- Submit button (.login-btn)
- Error message (#error)
";

var test = await generator.GenerateTestAsync(description);
```

Output:
```csharp
[Test]
public async Task ValidLoginTest() {
    await usernameField.FillAsync("test@example.com");
    await passwordField.FillAsync("password123");
    await submitButton.ClickAsync();
    
    await Expect(errorMessage).ToBeHiddenAsync();
}
```

## Complex Form
```csharp
var description = @"
Registration form:
- Personal info section
- Address fields
- Payment details
- Submit button
";

var options = new TestGenerationOptions {
    IncludeDataModels = true,
    TestFramework = "xUnit"
};

var test = await generator.GenerateTestAsync(
    new TestGenerationRequestBuilder()
        .WithPageDescription(description)
        .WithOptions(options)
        .Build()
);
```

## Data-Driven Tests
```csharp
var description = "Product listing with filtering";
var templateOptions = new TemplateOptions {
    TemplatePath = "DataDrivenTest.hbs"
};

var test = await generator.GenerateTestAsync(
    new TestGenerationRequestBuilder()
        .WithPageDescription(description)
        .WithContext("dataSource", "products.json")
        .Build()
);
```

---

# Best Practices

## Prompt Design
- Use clear page descriptions
- Include element relationships
- Specify workflow dependencies
- Define validation criteria

## Test Organization
- Group related tests
- Reuse setup/teardown
- Maintainable selectors
- Clear test names

## Performance Tips
- Enable template caching
- Batch test generation
- Reuse prompts
- Stream responses

## Error Prevention
- Validate page descriptions
- Test selectors
- Handle timeouts
- Check assertions

## Test Data
- Isolate test data
- Use environment variables
- Separate configs
- Document dependencies

