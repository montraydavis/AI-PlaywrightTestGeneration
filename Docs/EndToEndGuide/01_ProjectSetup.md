# Project Setup

## Requirements
- .NET 8.0 SDK
- Visual Studio 2022 or VS Code
- Ollama with LLama model

## Create Solution
In Visual Studio 2022:
1. `File > New > Project`
2. Select "Console App"
3. Name: PlaywrightTestGenerator

## NuGet Packages
```powershell
Install-Package Handlebars.Net
Install-Package Microsoft.Extensions.AI
Install-Package Microsoft.Extensions.AI.Ollama
Install-Package Microsoft.Extensions.Hosting
```

## Project Structure

### Models/
Core domain models - In Solution Explorer, add new class files:
- `PageElement.cs` - UI elements and selectors
- `TestCase.cs` - Test steps and assertions
- `UserTask.cs` - User workflows and actions

### Services/
Core components:
- `OllamaChatService.cs` - LLM integration
- `TemplateService.cs` - Code generation
- `TestGenerator.cs` - Test creation orchestrator

### Prompts/
LLM instructions:
- `ElementExtraction.md` - UI analysis prompts
- `TaskExtraction.md` - Workflow analysis prompts  
- `TestStructureGeneration.md` - Test structure prompts

### Templates/
Code templates:
- `CSTest.hbs` - NUnit test template

Ready to implement Models?