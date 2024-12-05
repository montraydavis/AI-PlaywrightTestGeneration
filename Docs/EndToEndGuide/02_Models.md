# Implementing Models for Test Generation

## Model Hierarchy Overview

Our test generator breaks down web pages into a structured hierarchy that mirrors how real testing works. Here's how each layer builds upon the previous:

1. UI Elements Layer (`PageElement`)
   - Represents individual UI components
   - Examples: buttons, input fields, dropdowns
   - Contains selectors for finding elements
   - Foundation for all interactions

2. Action Layer (`TaskStep`)
   - Single interaction with a UI element
   - Examples: clicking a button, entering text
   - Links to specific PageElement
   - Building block for workflows

3. Workflow Layer
   - `UserTask`: Real user behaviors
     - Example: "Log in to system"
     - Multiple TaskSteps in sequence
     - Business logic focused
   
   - `TestCase`: Verification scenarios
     - Example: "Verify login with valid credentials"
     - Setup, steps, assertions, cleanup
     - Testing logic focused

4. Organization Layer (`TestStructure`)
   - Contains all components
   - Manages relationships
   - Organizes test suites
   - Used for code generation

5. Configuration Layer
   - `TestGenerationRequest`: Input configuration
   - `TestGenerationOptions`: Generation settings
   - Controls test output format and behavior

---

## High-Level Component Structure

1. Base Components
   - `PageElement`: UI element definition (buttons, inputs)
   - `TaskStep`: Single interaction (click, input)

2. Workflow Components  
   - `UserTask`: Complete workflow definition
   - `TestCase`: Test definition with assertions

3. Container
   - `TestStructure`: Test suite container

4. Configuration
   - `TestGenerationRequest`: Input configuration
   - `TestGenerationOptions`: Generation settings

---

Relationships:

```
PageElement: UI Component
    │
    ├─► TaskStep: Single Action
    │       │
    │       ├─► UserTask: Workflow
    │       │       └─► TestStructure: Suite
    │       │
    │       └─► TestCase: Verification
    │               └─► TestStructure: Suite
    │
    ├─► TestStructure: Suite
    │
    └─► TestGenerationRequest
            └─► TestGenerationOptions
```

---

Let's implement each model with detailed explanations.

## 1. PageElement - UI Building Block

```csharp
public class PageElement
{
    public string Name { get; set; }
    public string Selector { get; set; }
    public string Type { get; set; }
    public string Description { get; set; }
    public Dictionary<string, string> Properties { get; set; } = new();
}
```

Key aspects:
- `Name`: Identifier used in test code (e.g., "loginButton")
- `Selector`: Playwright locator strategy
  - ID: `#login-button`
  - CSS: `.btn-primary`
  - Text: `text=Login`
- `Type`: Element category (button, input, link)
- `Properties`: Additional attributes
  - Validation rules
  - Default values
  - Custom behaviors

Example usage:
```csharp
var loginButton = new PageElement {
    Name = "loginButton",
    Selector = "#login-btn",
    Type = "button",
    Properties = new() {
        { "validation", "required" },
        { "testId", "login-submit" }
    }
};
```

## 2. TaskStep - Individual UI Interaction

```csharp
public class TaskStep
{
    public string Description { get; set; }    
    public string ElementName { get; set; }    
    public string Action { get; set; }         
    public Dictionary<string, string> Parameters { get; set; } = new();
}
```

TaskStep represents a single interaction with a PageElement:

### Properties Explained
- `Description`: Human-readable step explanation
- `ElementName`: References a PageElement by Name
- `Action`: Playwright action to perform
  - `click`: Button clicks
  - `fill`: Input text
  - `check`: Checkboxes
  - `select`: Dropdowns
- `Parameters`: Action-specific values
  - Text for input fields
  - Options for dropdowns
  - Custom data attributes

### Common Actions
```csharp
// Click action
var clickLogin = new TaskStep {
    Description = "Click login button",
    ElementName = "loginButton",
    Action = "click"
};

// Fill text
var enterUsername = new TaskStep {
    Description = "Enter username",
    ElementName = "usernameField",
    Action = "fill",
    Parameters = new() {
        { "value", "testuser@example.com" }
    }
};
```

### Usage in Test Generation
1. LLM identifies required actions
2. TaskStep gets created for each action
3. Steps combine into UserTasks/TestCases


## 3. UserTask - Complete Workflow

```csharp
public class UserTask
{
    public string Name { get; set; }
    public string Description { get; set; }
    public List<TaskStep> Steps { get; set; } = new();
    public List<string> Prerequisites { get; set; } = new();
    public List<string> ExpectedResults { get; set; } = new();
}
```

UserTask represents a complete user interaction flow:

### Core Components
- `Name`: Workflow identifier (e.g., "LoginFlow")
- `Steps`: Ordered list of TaskSteps
- `Prerequisites`: Required conditions
- `ExpectedResults`: Success criteria

### Example: Login Flow
```csharp
var loginFlow = new UserTask {
    Name = "StandardLogin",
    Description = "Log in with valid credentials",
    Prerequisites = new() {
        "User has valid account",
        "User is not logged in"
    },
    Steps = new() {
        new TaskStep {
            Description = "Enter username",
            ElementName = "usernameInput",
            Action = "fill",
            Parameters = new() { {"value", "user@example.com"} }
        },
        new TaskStep {
            Description = "Enter password",
            ElementName = "passwordInput",
            Action = "fill",
            Parameters = new() { {"value", "password123"} }
        },
        new TaskStep {
            Description = "Click login",
            ElementName = "loginButton",
            Action = "click"
        }
    },
    ExpectedResults = new() {
        "User redirected to dashboard",
        "Welcome message displayed"
    }
};
```

## 4. TestCase - Test Definition

```csharp
public class TestCase
{
    public string Name { get; set; }
    public string Description { get; set; }
    public List<string> Tags { get; set; } = new();
    public List<TaskStep> Setup { get; set; } = new();
    public List<TaskStep> Steps { get; set; } = new();
    public List<TaskStep> Assertions { get; set; } = new();
    public List<TaskStep> Cleanup { get; set; } = new();
}
```

### Key Features
- `Tags`: Test categorization and filtering
- `Setup`: Precondition steps
- `Steps`: Main test actions
- `Assertions`: Verification steps
- `Cleanup`: State restoration

### Example: Valid Login Test
```csharp
var loginTest = new TestCase {
    Name = "ValidLogin_ShouldSucceed",
    Description = "Verify successful login with valid credentials",
    Tags = new() { "smoke", "auth" },
    
    Setup = new() {
        new TaskStep {
            Description = "Clear cookies",
            ElementName = "browser",
            Action = "clearCookies"
        }
    },
    
    Steps = new() {
        new TaskStep {
            Description = "Enter valid username",
            ElementName = "usernameInput",
            Action = "fill",
            Parameters = new() { {"value", "test@example.com"} }
        },
        new TaskStep {
            Description = "Enter valid password",
            ElementName = "passwordInput",
            Action = "fill",
            Parameters = new() { {"value", "Password123!"} }
        },
        new TaskStep {
            Description = "Click login button",
            ElementName = "loginButton",
            Action = "click"
        }
    },
    
    Assertions = new() {
        new TaskStep {
            Description = "Verify dashboard URL",
            ElementName = "page",
            Action = "hasURL",
            Parameters = new() { {"value", "/dashboard"} }
        },
        new TaskStep {
            Description = "Verify welcome message",
            ElementName = "welcomeMessage",
            Action = "hasText",
            Parameters = new() { {"value", "Welcome back"} }
        }
    },
    
    Cleanup = new() {
        new TaskStep {
            Description = "Logout",
            ElementName = "logoutButton",
            Action = "click"
        }
    }
};
```

## 5. TestStructure - Test Suite Container

```csharp
public class TestStructure
{
    public string PageName { get; set; }
    public List<PageElement> Elements { get; set; } = new();
    public List<UserTask> Tasks { get; set; } = new();
    public List<TestCase> TestCases { get; set; } = new();
}
```

### Purpose
- Central container for test suite
- Organizes components hierarchically 
- Generated from LLM analysis
- Used by template engine

### Example: Login Page Test Suite
```csharp
var loginPageTests = new TestStructure {
    PageName = "LoginPage",
    
    Elements = new() {
        new PageElement {
            Name = "usernameInput",
            Selector = "#username",
            Type = "input"
        },
        new PageElement {
            Name = "loginButton",
            Selector = "#login-btn",
            Type = "button"
        }
    },
    
    Tasks = new() {
        new UserTask {
            Name = "BasicLogin",
            Steps = new() {
                new TaskStep { 
                    ElementName = "usernameInput",
                    Action = "fill"
                },
                new TaskStep {
                    ElementName = "loginButton",
                    Action = "click"
                }
            }
        }
    },
    
    TestCases = new() {
        new TestCase {
            Name = "ValidLogin_ShouldSucceed",
            Steps = new() { /* steps */ },
            Assertions = new() { /* assertions */ }
        }
    }
};
```

### Implementation Notes
1. Used by code generator for test file creation
2. Maintains relationships between components
3. Allows for test organization and filtering
4. Supports template-based generation

## 6. Configuration Models

```csharp
public class TestGenerationRequest
{
    public string PageDescription { get; set; }
    public Dictionary<string, string> AdditionalContext { get; set; } = new();
    public TestGenerationOptions Options { get; set; } = new();
}

public class TestGenerationOptions
{
    public bool IncludeComments { get; set; } = true;
    public bool GenerateDataModels { get; set; } = true;
    public string TestFramework { get; set; } = "NUnit";
    public string OutputPath { get; set; }
}
```

### Usage Example
```csharp
var request = new TestGenerationRequest {
    PageDescription = @"
        Login page with:
        - Username input (#username)
        - Password input (#password)
        - Login button (.login-btn)
    ",
    AdditionalContext = new() {
        { "environment", "staging" },
        { "baseUrl", "https://staging.app.com" }
    },
    Options = new() {
        IncludeComments = true,
        TestFramework = "NUnit",
        OutputPath = "Generated/LoginTests.cs"
    }
};
```

### Key Features
1. Request Configuration
   - Page description input
   - Environment context
   - Custom settings

2. Generation Options
   - Framework selection
   - Code style preferences
   - Output location
