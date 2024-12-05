using Microsoft.Extensions.AI;
using PlaywrightTestGenerator.PromptLoaders;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace PlaywrightTestGenerator.PromptEngines
{
    public class CSPlaywrightTestBuilderChainOfThought : IChainOfThoughtPromptEngine
    {
        private readonly IChatClient _chatClient;
        private readonly IPromptLoader _promptLoader;

        public CSPlaywrightTestBuilderChainOfThought(IChatClient chatClient, IPromptLoader promptLoader)
        {
            _chatClient = chatClient;
            _promptLoader = promptLoader;
        }

        private async Task<string> GetSystemPromptAsync(string name) =>
            await _promptLoader.LoadPromptAsync(name);

        public async Task<TestStructure> ProcessAsync(string prompt, CancellationToken cancellationToken = default)
        {
            var elements = await ExtractElementsAsync(prompt, cancellationToken);
            var tasks = await ExtractTasksAsync(prompt, elements, cancellationToken);
            return await GenerateTestStructureAsync(prompt, elements, tasks, cancellationToken);
        }
        private async Task<List<PageElement>> ExtractElementsAsync(string prompt, CancellationToken cancellationToken)
        {
            var messages = new List<ChatMessage>
            {
                new(ChatRole.System, await GetSystemPromptAsync("ElementExtraction")),
                new(ChatRole.User, prompt)
            };

            var response = await _chatClient.CompleteAsync(messages, cancellationToken: cancellationToken);
            return ParseElementsResponse(response.Message.Text
                ?? throw new Exception("Did not receive response from AI."));
        }

        private async Task<List<UserTask>> ExtractTasksAsync(string prompt, List<PageElement> elements, CancellationToken cancellationToken)
        {
            var elementsContext = JsonSerializer.Serialize(elements, new JsonSerializerOptions { WriteIndented = true });
            var messages = new List<ChatMessage>
            {
                new(ChatRole.System, await GetSystemPromptAsync("TaskExtraction")),
                new(ChatRole.User, $"Page Description:\n{prompt}\n\nAvailable Elements:\n{elementsContext}")
            };

            var response = await _chatClient.CompleteAsync(messages, cancellationToken: cancellationToken);
            return ParseTasksResponse(response.Message.Text
                ?? throw new Exception("Did not receive response from AI."));
        }

        private async Task<TestStructure> GenerateTestStructureAsync(string prompt, List<PageElement> elements, List<UserTask> tasks, CancellationToken cancellationToken)
        {
            var context = CreateTestGenerationContext(prompt, elements, tasks);
            var messages = new List<ChatMessage>
            {
                new(ChatRole.System, await GetSystemPromptAsync("TestStructureGeneration")),
                new(ChatRole.User, context)
            };

            var response = await _chatClient.CompleteAsync(messages, cancellationToken: cancellationToken);
            return ParseTestStructureResponse(response.Message.Text
                ?? throw new Exception("Did not receive response from AI."), 
                elements, 
                tasks);
        }

        private const string ElementExtractionSystemPrompt = @"You are a Playwright test automation expert. 
Extract UI elements from the page description. Focus on identifying:
1. Elements that can be interacted with (buttons, inputs, dropdowns)
2. Elements that display information (labels, text, tables)
3. Elements for validation (error messages, success notifications)

Provide the response in JSON format with the following structure:
{
    ""elements"": [
        {
            ""name"": ""string"",
            ""selector"": ""string"",
            ""type"": ""string"",
            ""description"": ""string""
        }
    ]
}

Use appropriate selectors (id, css, xpath) based on the context.
Do NOT include any commentary, instructions, or additional text in your response. Do not fend JSON code.";

        private const string TaskExtractionSystemPrompt = @"You are a Playwright test automation expert.
Extract user tasks and workflows from the page description, considering the available elements.
Focus on:
1. Common user interactions
2. Business workflows
3. Validation scenarios
4. Error scenarios

Provide the response in JSON format with the following structure:
{
    ""tasks"": [
        {
            ""name"": ""string"",
            ""description"": ""string"",
            ""steps"": [
                {
                    ""description"": ""string"",
                    ""elementName"": ""string"",
                    ""action"": ""string"",
                    ""parameters"": {
                        ""key"": ""value""
                    }
                }
            ],
            ""prerequisites"": [""string""],
            ""expectedResults"": [""string""]
        }
    ]
}

Do NOT include any commentary, instructions, or additional text in your response. Do not fend JSON code.";

        private const string TestStructureGenerationSystemPrompt = @"You are a Playwright test automation expert.
Generate a complete test structure considering the page description, elements, and tasks.
Focus on:
1. Test case organization
2. Setup and cleanup steps
3. Assertions and validations
4. Error handling

Provide the response in JSON format matching the TestStructure model.

Do NOT include any commentary, instructions, or additional text in your response. Do not fend JSON code.";

        private List<PageElement> ParseElementsResponse(string content)
        {
            try
            {
                var response = JsonSerializer.Deserialize<ElementsResponse>(content);
                return response?.Elements ?? new List<PageElement>();
            }
            catch (JsonException)
            {
                return new List<PageElement>();
            }
        }

        private List<UserTask> ParseTasksResponse(string content)
        {
            try
            {
                var response = JsonSerializer.Deserialize<TasksResponse>(content);
                return response?.Tasks ?? new List<UserTask>();
            }
            catch (JsonException)
            {
                return new List<UserTask>();
            }
        }

        private TestStructure ParseTestStructureResponse(string content, List<PageElement> elements, List<UserTask> tasks)
        {
            try
            {
                var structure = JsonSerializer.Deserialize<TestStructure>(content)
                    ?? throw new Exception($"Failed to deserialize `{nameof(TestStructure)}`.");
                structure.Elements = elements;
                structure.Tasks = tasks;
                return structure;
            }
            catch (JsonException)
            {
                return new TestStructure
                {
                    Elements = elements,
                    Tasks = tasks
                };
            }
        }

        private string SerializeElements(List<PageElement> elements)
        {
            return JsonSerializer.Serialize(elements, new JsonSerializerOptions
            {
                WriteIndented = true
            });
        }

        private string CreateTestGenerationContext(string prompt, List<PageElement> elements, List<UserTask> tasks)
        {
            return $@"Page Description:
{prompt}

Available Elements:
{SerializeElements(elements)}

Available Tasks:
{JsonSerializer.Serialize(tasks, new JsonSerializerOptions { WriteIndented = true })}";
        }

        public class ElementsResponse
        {
            [JsonPropertyName("elements")]
            public List<PageElement> Elements { get; set; } = new();
        }

        private class TasksResponse
        {

            [JsonPropertyName("tasks")]
            public List<UserTask> Tasks { get; set; } = new();
        }
    }
}
