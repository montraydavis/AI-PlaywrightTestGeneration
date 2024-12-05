# OllamaChatService Implementation

## Overview
Provides LLM integration via Ollama for test generation through a standardized chat interface.

## Interface
```csharp
public interface IChatClient 
{
    Task<ChatCompletion> CompleteAsync(
        IList<ChatMessage> messages,
        ChatOptions options = null,
        CancellationToken cancellationToken = default);

    IAsyncEnumerable<StreamingChatCompletionUpdate> CompleteStreamingAsync(
        IList<ChatMessage> messages,
        ChatOptions options = null,
        CancellationToken cancellationToken = default);
}
```

## Core Implementation
```csharp
public class OllamaChatService : IChatClient
{
    private readonly ILogger<OllamaChatService> _logger;
    private readonly IChatClient _ollamaClient;
    private readonly ChatClientMetadata _metadata;
    private readonly ChatOptions _defaultOptions;
    private bool _disposed;

    public OllamaChatService(
        ILogger<OllamaChatService> logger,
        IChatClient ollamaClient,
        string modelId = "llama2",
        ChatOptions defaultOptions = null)
    {
        _logger = logger;
        _ollamaClient = ollamaClient;
        _metadata = new ChatClientMetadata 
        { 
            ModelId = modelId,
            SupportsStreaming = true
        };
        _defaultOptions = defaultOptions ?? new ChatOptions
        {
            ModelId = modelId,
            Temperature = 0.1,
            MaxOutputTokens = 4096
        };
    }

    public async Task<ChatCompletion> CompleteAsync(
        IList<ChatMessage> messages,
        ChatOptions options = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var requestOptions = options ?? _defaultOptions;
            ValidateRequest(messages, requestOptions);

            _logger.LogDebug("Sending completion request with {Count} messages", 
                messages.Count);

            var response = await _ollamaClient.CompleteAsync(
                messages, 
                requestOptions, 
                cancellationToken);

            _logger.LogDebug("Received response with {Length} characters",
                response?.Message?.Text?.Length ?? 0);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during chat completion");
            throw;
        }
    }
}
```

## Key Features

1. Request Validation
```csharp
private void ValidateRequest(IList<ChatMessage> messages, ChatOptions options)
{
    if (messages == null || messages.Count == 0)
        throw new ArgumentException("Messages cannot be empty");

    if (messages.Count > MaxMessagesPerRequest)
        throw new ArgumentException($"Maximum {MaxMessagesPerRequest} messages per request");

    ValidateMessageContents(messages);
    ValidateOptions(options);
}
```

2. Configuration & Settings
```csharp
private ChatOptions CreateRequestOptions(ChatOptions userOptions)
{
    return new ChatOptions
    {
        ModelId = userOptions?.ModelId ?? _defaultOptions.ModelId,
        Temperature = userOptions?.Temperature ?? _defaultOptions.Temperature,
        MaxOutputTokens = userOptions?.MaxOutputTokens ?? _defaultOptions.MaxOutputTokens,
        StopSequences = userOptions?.StopSequences
    };
}
```

3. Error Handling & Recovery
```csharp
private async Task<ChatCompletion> RetryWithFallback(
    IList<ChatMessage> messages,
    ChatOptions options,
    CancellationToken cancellationToken)
{
    var retryCount = 0;
    while (retryCount < MaxRetries)
    {
        try
        {
            return await _ollamaClient.CompleteAsync(
                messages, options, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, 
                "Retry {Count} failed", retryCount + 1);
            retryCount++;
            await Task.Delay(RetryDelay * retryCount);
        }
    }
    throw new ChatCompletionException("Max retries exceeded");
}
```

Continue with HandlebarsTemplateService?