# Builder Pattern Implementation

## Overview

The builder pattern creates test components through a fluent, validated interface. Each builder:
- Validates required properties
- Ensures logical relationships 
- Provides clear error messages
- Uses method chaining

## PageElementBuilder
Constructs UI element definitions with validation:

```csharp
public class PageElementBuilder 
{
    private readonly PageElement _element = new();
    private readonly List<string> _validationErrors = new();

    // Core Properties
    public PageElementBuilder WithName(string name) 
    {
        ValidateNotEmpty(name, "Element name");
        _element.Name = name;
        return this;
    }

    public PageElementBuilder WithSelector(string selector)
    {
        ValidateSelector(selector);
        _element.Selector = selector;
        return this;
    }

    // Element Configuration
    public PageElementBuilder WithType(ElementType type)
    {
        _element.Type = type.ToString();
        return this;
    }

    public PageElementBuilder WithDescription(string description)
    {
        _element.Description = description;
        return this;
    }

    public PageElementBuilder WithValidation(string rule)
    {
        _element.Properties["validation"] = rule;
        return this;
    }

    // Validation Logic
    private void ValidateNotEmpty(string value, string field)
    {
        if (string.IsNullOrEmpty(value))
            _validationErrors.Add($"{field} cannot be empty");
    }

    public PageElement Build()
    {
        ValidateElement();
        return _element;
    }
}
```

### Key Features
- Name validation
- Selector validation (ID/CSS)
- Element type categorization  
- Property management
- Build validation

### Example Usage
```csharp
var loginButton = new PageElementBuilder()
    .WithName("loginButton")
    .WithSelector("#login")
    .WithType(ElementType.Button)
    .WithDescription("Primary login button")
    .WithValidation("required")
    .Build();
```

## TaskStepBuilder
Constructs individual test actions:

```csharp
public class TaskStepBuilder
{
    private readonly TaskStep _step = new();
    private readonly List<string> _validationErrors = new();

    // Core Action Properties
    public TaskStepBuilder WithDescription(string description)
    {
        _step.Description = description;
        return this;
    }

    public TaskStepBuilder WithElement(string elementName)
    {
        ValidateNotEmpty(elementName, "Element name");
        _step.ElementName = elementName;
        return this;
    }

    public TaskStepBuilder WithAction(ActionType action)
    {
        _step.Action = action.ToString();
        return this;
    }

    // Action Parameters
    public TaskStepBuilder WithValue(string value)
    {
        _step.Parameters["value"] = value;
        return this;
    }

    public TaskStepBuilder WithTimeout(int milliseconds)
    {
        _step.Parameters["timeout"] = milliseconds.ToString();
        return this;
    }

    public TaskStepBuilder WithParameter(string key, string value)
    {
        _step.Parameters[key] = value;
        return this;
    }
}

public enum ActionType
{
    Click,
    Fill,
    Check,
    Uncheck,
    Select,
    Press,
    Type,
    Hover,
    Focus,
    WaitFor
}
```

### Key Features
- Element reference validation
- Action type enforcement
- Parameter management
- Timeout configuration
- Custom parameters

### Example Usage
```csharp
var loginAction = new TaskStepBuilder()
    .WithDescription("Enter credentials and login")
    .WithElement("usernameInput")
    .WithAction(ActionType.Fill)
    .WithValue("test@example.com")
    .WithTimeout(5000)
    .Build();

var submitAction = new TaskStepBuilder()
    .WithElement("loginButton") 
    .WithAction(ActionType.Click)
    .Build();
```

## UserTaskBuilder
Constructs workflow definitions:

```csharp
public class UserTaskBuilder
{
    private readonly UserTask _task = new();
    private readonly List<string> _validationErrors = new();

    public UserTaskBuilder WithName(string name)
    {
        ValidateNotEmpty(name, "Task name");
        _task.Name = name;
        return this;
    }

    public UserTaskBuilder WithDescription(string description)
    {
        _task.Description = description;
        return this;
    }

    public UserTaskBuilder AddStep(Action<TaskStepBuilder> stepConfig)
    {
        var builder = new TaskStepBuilder();
        stepConfig(builder);
        _task.Steps.Add(builder.Build());
        return this;
    }

    public UserTaskBuilder AddPrerequisite(string prerequisite)
    {
        _task.Prerequisites.Add(prerequisite);
        return this;
    }

    public UserTaskBuilder AddExpectedResult(string result)
    {
        _task.ExpectedResults.Add(result);
        return this;
    }

    public UserTask Build()
    {
        ValidateTask();
        return _task;
    }
}
```

### Key Features
- Name/description management
- Step configuration using TaskStepBuilder
- Prerequisites tracking
- Expected results definition
- Task validation

### Example Usage
```csharp
var loginTask = new UserTaskBuilder()
    .WithName("LoginFlow")
    .WithDescription("Standard user login")
    .AddPrerequisite("User exists")
    .AddStep(step => step
        .WithElement("usernameInput")
        .WithAction(ActionType.Fill)
        .WithValue("test@example.com"))
    .AddStep(step => step
        .WithElement("loginButton")
        .WithAction(ActionType.Click))
    .AddExpectedResult("User is logged in")
    .Build();
```

## TestCaseBuilder
Constructs verification test cases:

```csharp
public class TestCaseBuilder
{
    private readonly TestCase _testCase = new();
    private readonly List<string> _validationErrors = new();

    public TestCaseBuilder WithName(string name)
    {
        ValidateNotEmpty(name, "Test case name");
        _testCase.Name = name;
        return this;
    }

    public TestCaseBuilder WithDescription(string description)
    {
        _testCase.Description = description;
        return this;
    }

    public TestCaseBuilder AddTag(string tag)
    {
        _testCase.Tags.Add(tag);
        return this;
    }

    public TestCaseBuilder AddSetup(Action<TaskStepBuilder> setupConfig)
    {
        var builder = new TaskStepBuilder();
        setupConfig(builder);
        _testCase.Setup.Add(builder.Build());
        return this;
    }

    public TestCaseBuilder AddStep(Action<TaskStepBuilder> stepConfig)
    {
        var builder = new TaskStepBuilder();
        stepConfig(builder);
        _testCase.Steps.Add(builder.Build());
        return this;
    }

    public TestCaseBuilder AddAssertion(Action<TaskStepBuilder> assertionConfig)
    {
        var builder = new TaskStepBuilder();
        assertionConfig(builder);
        _testCase.Assertions.Add(builder.Build());
        return this;
    }

    public TestCaseBuilder AddCleanup(Action<TaskStepBuilder> cleanupConfig)
    {
        var builder = new TaskStepBuilder();
        cleanupConfig(builder);
        _testCase.Cleanup.Add(builder.Build());
        return this;
    }
}
```

### Key Features
- Test case metadata management
- Setup steps configuration
- Test steps definition
- Assertion handling
- Cleanup steps management

### Example Usage
```csharp
var loginTest = new TestCaseBuilder()
    .WithName("ValidLogin_ShouldSucceed")
    .WithDescription("Verify successful login")
    .AddTag("smoke")
    .AddTag("auth")
    .AddSetup(setup => setup
        .WithElement("browser")
        .WithAction(ActionType.ClearCookies))
    .AddStep(step => step
        .WithElement("usernameInput")
        .WithAction(ActionType.Fill)
        .WithValue("test@example.com"))
    .AddAssertion(assert => assert
        .WithElement("welcomeMessage")
        .WithAction(ActionType.HasText)
        .WithValue("Welcome"))
    .Build();
```

## TestStructureBuilder
Manages suite composition:

```csharp
public class TestStructureBuilder
{
    private readonly TestStructure _structure = new();
    private readonly List<string> _validationErrors = new();

    public TestStructureBuilder WithPageName(string name)
    {
        ValidateNotEmpty(name, "Page name");
        _structure.PageName = name;
        return this;
    }

    public TestStructureBuilder AddElement(Action<PageElementBuilder> elementConfig)
    {
        var builder = new PageElementBuilder();
        elementConfig(builder);
        _structure.Elements.Add(builder.Build());
        return this;
    }

    public TestStructureBuilder AddTask(Action<UserTaskBuilder> taskConfig)
    {
        var builder = new UserTaskBuilder();
        taskConfig(builder);
        _structure.Tasks.Add(builder.Build());
        return this;
    }

    public TestStructureBuilder AddTestCase(Action<TestCaseBuilder> testConfig)
    {
        var builder = new TestCaseBuilder();
        testConfig(builder);
        _structure.TestCases.Add(builder.Build());
        return this;
    }
}
```

### Key Features
- Page identification
- Element registration
- Task management
- Test case organization

### Example Usage
```csharp
var loginPage = new TestStructureBuilder()
    .WithPageName("LoginPage")
    .AddElement(e => e
        .WithName("loginButton")
        .WithSelector("#login")
        .WithType(ElementType.Button))
    .AddTask(t => t
        .WithName("StandardLogin")
        .AddStep(s => s
            .WithElement("loginButton")
            .WithAction(ActionType.Click)))
    .AddTestCase(tc => tc
        .WithName("ValidLogin")
        .AddStep(s => s
            .WithElement("loginButton")
            .WithAction(ActionType.Click)))
    .Build();
```

## TestGenerationRequestBuilder
Creates and validates test generation configuration:

```csharp
public class TestGenerationRequestBuilder
{
    private readonly TestGenerationRequest _request = new();
    private readonly List<string> _validationErrors = new();

    public TestGenerationRequestBuilder WithPageDescription(string description)
    {
        ValidateNotEmpty(description, "Page description");
        _request.PageDescription = description;
        return this;
    }

    public TestGenerationRequestBuilder WithEnvironment(string env)
    {
        ValidateEnvironment(env);
        _request.AdditionalContext["environment"] = env;
        return this;
    }

    public TestGenerationRequestBuilder WithBaseUrl(string url)
    {
        ValidateUrl(url);
        _request.AdditionalContext["baseUrl"] = url;
        return this;
    }

    public TestGenerationRequestBuilder WithOptions(Action<TestGenerationOptionsBuilder> configure)
    {
        var builder = new TestGenerationOptionsBuilder();
        configure(builder);
        _request.Options = builder.Build();
        return this;
    }
}

public class TestGenerationOptionsBuilder
{
    private readonly TestGenerationOptions _options = new();

    public TestGenerationOptionsBuilder WithFramework(string framework)
    {
        _options.TestFramework = framework;
        return this;
    }

    public TestGenerationOptionsBuilder WithOutputPath(string path)
    {
        _options.OutputPath = path;
        return this;
    }

    public TestGenerationOptionsBuilder IncludeComments(bool include = true)
    {
        _options.IncludeComments = include;
        return this;
    }
}
```

### Key Features
- Page description validation
- Environment configuration
- URL validation
- Framework selection
- Output path management

### Example Usage
```csharp
var request = new TestGenerationRequestBuilder()
    .WithPageDescription(@"
        Login page with:
        - Username field
        - Password field
        - Login button
    ")
    .WithEnvironment("staging")
    .WithBaseUrl("https://staging.app.com")
    .WithOptions(opt => opt
        .WithFramework("NUnit")
        .WithOutputPath("Tests/Generated")
        .IncludeComments())
    .Build();
```

This completes our builder implementations. Ready to move on to the next section?