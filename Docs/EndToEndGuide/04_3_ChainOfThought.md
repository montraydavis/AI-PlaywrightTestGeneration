# CSPlaywrightTestBuilderChainOfThought Service

## Core Implementation
```csharp
public class CSPlaywrightTestBuilderChainOfThought : IChainOfThoughtPromptEngine
{
    private readonly IChatClient _chatClient;
    private readonly IPromptLoader _promptLoader;
    private readonly ILogger<CSPlaywrightTestBuilderChainOfThought> _logger;

    public async Task<TestStructure> ProcessAsync(
        string prompt, 
        CancellationToken cancellationToken = default)
    {
        var elements = await ExtractElementsAsync(prompt, cancellationToken);
        var tasks = await ExtractTasksAsync(prompt, elements, cancellationToken);
        return await GenerateTestStructureAsync(prompt, elements, tasks, cancellationToken);
    }
}
```

## Phase Processing

### 1. Element Extraction
```csharp
private async Task<List<PageElement>> ExtractElementsAsync(
    string prompt, 
    CancellationToken cancellationToken)
{
    var elementPrompt = await _promptLoader.LoadPromptAsync("ElementExtraction");
    var messages = new List<ChatMessage>
    {
        new(ChatRole.System, elementPrompt),
        new(ChatRole.User, prompt)
    };

    var response = await _chatClient.CompleteAsync(messages, cancellationToken: cancellationToken);
    return ParseElementsResponse(response.Message.Text);
}
```

### 2. Task Extraction 
```csharp
private async Task<List<UserTask>> ExtractTasksAsync(
    string prompt,
    List<PageElement> elements,
    CancellationToken cancellationToken)
{
    var taskPrompt = await _promptLoader.LoadPromptAsync("TaskExtraction");
    var context = SerializeElements(elements);
    var messages = new List<ChatMessage>
    {
        new(ChatRole.System, taskPrompt),
        new(ChatRole.User, $"{prompt}\n\nElements:\n{context}")
    };

    var response = await _chatClient.CompleteAsync(messages, cancellationToken: cancellationToken);
    return ParseTasksResponse(response.Message.Text);
}
```

### 3. Test Structure Generation
```csharp
private async Task<TestStructure> GenerateTestStructureAsync(
    string prompt,
    List<PageElement> elements,
    List<UserTask> tasks,
    CancellationToken cancellationToken)
{
    var structurePrompt = await _promptLoader.LoadPromptAsync("TestStructureGeneration");
    var context = CreateTestContext(prompt, elements, tasks);

    var response = await _chatClient.CompleteAsync(
        new List<ChatMessage>
        {
            new(ChatRole.System, structurePrompt),
            new(ChatRole.User, context)
        },
        cancellationToken: cancellationToken);

    return ParseTestStructure(response.Message.Text, elements, tasks);
}
```

## Response Parsing
```csharp
private TestStructure ParseTestStructure(
    string content, 
    List<PageElement> elements,
    List<UserTask> tasks)
{
    try
    {
        var structure = JsonSerializer.Deserialize<TestStructure>(content);
        structure.Elements = elements;
        structure.Tasks = tasks;
        return structure;
    }
    catch (JsonException ex)
    {
        _logger.LogError(ex, "Failed to parse test structure");
        throw new TestGenerationException("Invalid test structure", ex);
    }
}
```

This service orchestrates the entire test generation process through LLM interactions.