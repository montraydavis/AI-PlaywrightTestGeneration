using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PlaywrightTestGenerator.Exceptions;
using PlaywrightTestGenerator.PromptEngines;
using PlaywrightTestGenerator.PromptLoaders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace PlaywrightTestGenerator
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddOllamaServices(this IServiceCollection services, string baseUrl)
        {
            services.AddSingleton<IChatClient>(sp =>
                new OllamaChatClient(new Uri(baseUrl)));

            services.AddScoped<OllamaChatService>();

            return services;
        }

        public static IServiceCollection AddTestGeneration(
        this IServiceCollection services,
        Action<TestTemplateOptions>? configureTemplate = null)
        {
            // Configure template options
            if (configureTemplate != null)
            {
                services.Configure(configureTemplate);
            }
            else
            {
                services.Configure<TestTemplateOptions>(config =>
                {
                    config.TemplatesPath = "Templates";
                    config.UseTemplateCache = true;
                });
            }

            // Register services
            services.AddScoped<IChatClient>(sp =>
            {
                var baseClient = new OllamaChatClient(new Uri("http://localhost:11434/"), "llama3.1");
                return new OllamaChatService(sp, baseClient);
            });

            services.AddScoped<IChainOfThoughtPromptEngine, CSPlaywrightTestBuilderChainOfThought>();
            services.AddSingleton<ITemplateService, HandlebarsTemplateService>();

            return services;
        }
    }

    internal class Program
    {
        static async Task Main(string[] args)
        {
            //            var builder = Host.CreateApplicationBuilder(args);

            //            // Register services
            //            builder.Services.AddScoped<IChatClient>(sp =>
            //            {
            //                var baseClient = new OllamaChatClient(new Uri("http://localhost:11434/"), "phi3:mini");
            //                return new OllamaChatService(sp, baseClient);
            //            });
            //            builder.Services.AddScoped<IChainOfThoughtPromptEngine, CSPlaywrightTestBuilderChainOfThought>();

            //            using var host = builder.Build();

            //            // Get the chain of thought engine
            //            var promptEngine = host.Services.GetRequiredService<IChainOfThoughtPromptEngine>();

            //            // Example page description
            //            var pageDescription = @"
            //Login page for an e-commerce website:
            //- Username field with id 'username'
            //- Password field with id 'password'
            //- 'Remember me' checkbox with class 'remember-checkbox'
            //- Login button with text 'Sign In'
            //- Error message area with class 'error-message'
            //- 'Forgot Password' link

            //User should be able to:
            //1. Login with valid credentials
            //2. See error message for invalid credentials
            //3. Use remember me functionality
            //4. Reset password using forgot password link";

            //            try
            //            {
            //                // Generate test structure using chain of thought
            //                var testStructure = await promptEngine.ProcessAsync(pageDescription);

            //                // Print the generated structure
            //                Console.WriteLine($"Generated test structure for: {testStructure.PageName}");
            //                Console.WriteLine("\nElements:");
            //                foreach (var element in testStructure.Elements)
            //                {
            //                    Console.WriteLine($"- {element.Name}: {element.Selector} ({element.Type})");
            //                }

            //                Console.WriteLine("\nTasks:");
            //                foreach (var task in testStructure.Tasks)
            //                {
            //                    Console.WriteLine($"\n{task.Name}:");
            //                    foreach (var step in task.Steps)
            //                    {
            //                        Console.WriteLine($"- {step.Description}");
            //                    }
            //                }

            //                Console.WriteLine("\nTest Cases:");
            //                foreach (var testCase in testStructure.TestCases)
            //                {
            //                    Console.WriteLine($"\n{testCase.Name}:");
            //                    Console.WriteLine("Setup steps:");
            //                    foreach (var step in testCase.Setup)
            //                    {
            //                        Console.WriteLine($"- {step.Description}");
            //                    }
            //                    Console.WriteLine("Test steps:");
            //                    foreach (var step in testCase.Steps)
            //                    {
            //                        Console.WriteLine($"- {step.Description}");
            //                    }
            //                    Console.WriteLine("Assertions:");
            //                    foreach (var assertion in testCase.Assertions)
            //                    {
            //                        Console.WriteLine($"- {assertion.Description}");
            //                    }
            //                }
            //            }
            //            catch (Exception ex)
            //            {
            //                Console.WriteLine($"Error generating test structure: {ex.Message}");
            //            }

            var builder = Host.CreateApplicationBuilder(args);

            // Configure services
            builder.Services.Configure<TestTemplateOptions>(config =>
            {
                config.TemplatesPath = "Templates";
                config.UseTemplateCache = true;
                config.DefaultTemplateValues = new Dictionary<string, string>
                {
                    { "Ns", "PlaywrightTests" },
                    { "BaseUrl", "http://localhost:3000" }
                };
            });

            // Register services
            builder.Services.AddScoped<IChatClient>(sp =>
            {
                var baseClient = new OllamaChatClient(new Uri("http://localhost:11434/"), "llama3.1");
                return new OllamaChatService(sp, baseClient);
            });
            builder.Services.AddScoped<IPromptLoader, FilePromptLoader>();
            builder.Services.AddScoped<IChainOfThoughtPromptEngine, CSPlaywrightTestBuilderChainOfThought>();
            builder.Services.AddSingleton<ITemplateService, HandlebarsTemplateService>();

            using var host = builder.Build();

            try
            {
                // Get services
                var promptEngine = host.Services.GetRequiredService<IChainOfThoughtPromptEngine>();
                var templateService = host.Services.GetRequiredService<ITemplateService>();

                // Example page description
                var pageDescription = @"Login page for an e-commerce website:
- Username field with id 'username'
- Password field with id 'password'
- 'Remember me' checkbox with class 'remember-checkbox'
- Login button with text 'Sign In'
- Error message area with class 'error-message'
- 'Forgot Password' link

User should be able to:
1. Login with valid credentials
2. See error message for invalid credentials
3. Use remember me functionality
4. Reset password using forgot password link";

                // Generate test structure
                Console.WriteLine("Generating test structure...");
                var testStructure = await promptEngine.ProcessAsync(pageDescription);

                // Configure template options
                var templateOptions = new TemplateOptions
                {
                    Ns = "ExampleApp.LoginTests",
                    BaseUrl = "http://example.com",
                    TemplatePath = "CSTest.hbs"
                };

                // Generate test code
                Console.WriteLine("Generating test code...");
                var generatedCode = await templateService.RenderTestAsync(testStructure, templateOptions);

                // Save the generated code
                var outputPath = Path.Combine("Generated", "LoginTests.cs");
                Directory.CreateDirectory("Generated");
                await File.WriteAllTextAsync(outputPath, generatedCode);

                Console.WriteLine($"Test code generated successfully: {outputPath}");

                // Print summary
                Console.WriteLine("\nGenerated Test Structure:");
                Console.WriteLine($"Page: {testStructure.PageName}");
                Console.WriteLine($"\nElements: {testStructure.Elements.Count}");
                foreach (var element in testStructure.Elements)
                {
                    Console.WriteLine($"- {element.Name}: {element.Selector} ({element.Type})");
                }

                Console.WriteLine($"\nTasks: {testStructure.Tasks.Count}");
                foreach (var task in testStructure.Tasks)
                {
                    Console.WriteLine($"- {task.Name}");
                }

                Console.WriteLine($"\nTest Cases: {testStructure.TestCases.Count}");
                foreach (var testCase in testStructure.TestCases)
                {
                    Console.WriteLine($"- {testCase.Name}");
                }
            }
            catch (TemplateNotFoundException ex)
            {
                Console.WriteLine($"Template Error: {ex.Message}");
            }
            catch (TemplateRenderException ex)
            {
                Console.WriteLine($"Rendering Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected Error: {ex.Message}");
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
