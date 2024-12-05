using Microsoft.Extensions.AI;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace PlaywrightTestGenerator
{

    public class OllamaChatService : IChatClient
    {
        private bool _disposed;
        private IServiceProvider _serviceProvider;
        private IChatClient _chatClient;

        public OllamaChatService(IServiceProvider serviceProvider,
            IChatClient chatClient)
        {
            _serviceProvider = serviceProvider;
            _chatClient = chatClient;
        }

        public ChatClientMetadata Metadata { get; } = new ChatClientMetadata();

        public async Task<ChatCompletion> CompleteAsync(
            IList<ChatMessage> messages,
            ChatOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            var requestOptions = new ChatOptions
            {
                ModelId = "llama3.1",
                Temperature = 0,
                MaxOutputTokens = 100000
            };

            var response = await _chatClient.CompleteAsync(messages, options ?? requestOptions, cancellationToken);

            return response;
        }

        public IAsyncEnumerable<StreamingChatCompletionUpdate> CompleteStreamingAsync(
            IList<ChatMessage> messages,
            ChatOptions? options = null,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var requestOptions = new ChatOptions
            {
                ModelId = "llama3.1",
                Temperature = 0,
                MaxOutputTokens = 100000
            };

            var response = _chatClient.CompleteStreamingAsync(messages, options ?? requestOptions, cancellationToken);

            return response;
        }

        public void Dispose()
        {
            _chatClient.Dispose();
        }

        public object? GetService(Type serviceType, object? serviceKey = null) => null;
    }
}
