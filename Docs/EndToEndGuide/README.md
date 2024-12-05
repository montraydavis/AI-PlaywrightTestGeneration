# Playwright Test Generator Tutorial

This tutorial teaches you how to build an AI-powered test automation tool that converts page descriptions into Playwright tests. You'll learn to create a system that:

1. Understands UI structures using LLMs
2. Converts natural language to test scenarios
3. Generates production-ready test code

## What You'll Build

```csharp
// Input: Natural language description

A user authentication system with:
- Login form
  - Email field with validation
  - Password field (min 8 chars)
  - Remember me checkbox
  - Submit button
- Validation messages
  - Invalid email format
  - Incorrect credentials
  - Password requirements
- "Forgot password" link
- "Register" link for new users
```

```csharp
// Output: Test code

[TestFixture]
public class AuthenticationTests : PageTest 
{
    private ILocator emailField => _page.Locator("#email");
    private ILocator passwordField => _page.Locator("#password");
    private ILocator rememberMe => _page.Locator(".remember-checkbox");
    private ILocator submitButton => _page.Locator("button[type='submit']");
    private ILocator validationMessage => _page.Locator(".validation-message");
    private ILocator forgotPasswordLink => _page.Locator("a:text('Forgot password')");
    private ILocator registerLink => _page.Locator("a:text('Register')");

    [Test]
    public async Task ValidLogin_ShouldSucceed() {
        await emailField.FillAsync("test@example.com");
        await passwordField.FillAsync("Password123!");
        await rememberMe.CheckAsync();
        await submitButton.ClickAsync();
        
        await Expect(_page).ToHaveURLAsync("/dashboard");
    }

    [Test]
    public async Task InvalidEmail_ShouldShowError() {
        await emailField.FillAsync("invalid-email");
        await submitButton.ClickAsync();
        
        await Expect(validationMessage).ToHaveTextAsync("Please enter a valid email address");
    }

    [Test]
    public async Task ShortPassword_ShouldShowError() {
        await emailField.FillAsync("test@example.com");
        await passwordField.FillAsync("123");
        await submitButton.ClickAsync();
        
        await Expect(validationMessage).ToHaveTextAsync("Password must be at least 8 characters");
    }
}
```

## Core Principles

1. Architecture & Design
   - Chain-of-thought prompt engineering
   - Template-based code generation
   - Service-oriented architecture

2. Technologies
   - Ollama integration for LLM processing
   - Handlebars for test templates
   - Microsoft.Extensions.AI

3. Best Practices
   - Test structure organization
   - Prompt design principles
   - Error handling strategies
   - Performance optimization

4. Advanced Topics
   - Custom prompt engines
   - Template customization
   - Data-driven testing
   - Parallel test generation

## Prerequisites
- C# and .NET knowledge
- Basic understanding of Playwright
- Familiarity with testing concepts

## Table of Contents

1. Project Setup
   - Requirements
   - Project Structure
   - Dependencies

2. Models
   - PageElement
   - UserTask
   - TaskStep
   - TestCase
   - TestStructure
   - TestGenerationRequest
   - TestGenerationOptions

3. Support Classes
   - Custom Exceptions
     - TemplateNotFoundException
     - TemplateRenderException
     - TemplateConfigurationException
     - TestGenerationException
   - Builder Classes
     - TestGenerationRequestBuilder
     - TestStructureBuilder
     - PageElementBuilder
     - UserTaskBuilder
     - TaskStepBuilder
     - TestCaseBuilder

4. Services
   - IPromptLoader & FilePromptLoader
   - OllamaChatService
   - HandlebarsTemplateService
   - CSPlaywrightTestBuilderChainOfThought

5. Configuration
   - Service Registration
   - Template Configuration
   - Prompt System Setup

6. Usage Examples
   - Basic Test Generation
   - Custom Templates
   - Advanced Scenarios