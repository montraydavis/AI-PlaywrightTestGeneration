using HandlebarsDotNet;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PlaywrightTestGenerator.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace PlaywrightTestGenerator.PromptEngines
{
    public class HandlebarsTemplateService : ITemplateService, IDisposable
    {
        private readonly IHandlebars _handlebars;
        private readonly TestTemplateOptions _options;
        private readonly ILogger<HandlebarsTemplateService> _logger;
        private readonly Dictionary<string, HandlebarsTemplate<object, object>> _templateCache;
        private bool _disposed;

        public HandlebarsTemplateService(
            IOptions<TestTemplateOptions> options,
            ILogger<HandlebarsTemplateService> logger)
        {
            _options = options.Value;
            _logger = logger;
            _templateCache = new Dictionary<string, HandlebarsTemplate<object, object>>();

            _handlebars = Handlebars.Create(new HandlebarsConfiguration()
            {
                ThrowOnUnresolvedBindingExpression = true,
                NoEscape = false
            });

            RegisterHelpers();
        }

        public async Task<string> RenderTestAsync(TestStructure testStructure, TemplateOptions options)
        {
            try
            {
                _logger.LogInformation("Starting test code generation for page: {PageName}", testStructure.PageName);

                var template = await GetTemplateAsync(options.TemplatePath);
                var data = CreateTemplateData(testStructure, options);

                var result = template(data);

                _logger.LogInformation("Successfully generated test code for page: {PageName}", testStructure.PageName);
                return result;
            }
            catch (FileNotFoundException ex)
            {
                _logger.LogError(ex, "Template file not found: {TemplatePath}", options.TemplatePath);
                throw new TemplateNotFoundException($"Template file not found: {options.TemplatePath}", ex);
            }
            catch (HandlebarsException ex)
            {
                _logger.LogError(ex, "Error compiling or rendering template for page: {PageName}", testStructure.PageName);
                throw new TemplateRenderException("Error rendering template", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during test generation for page: {PageName}", testStructure.PageName);
                throw new TestGenerationException("Failed to generate test code", ex);
            }
        }


        private async Task<HandlebarsTemplate<object, object>> GetTemplateAsync(string templatePath)
        {
            var fullPath = Path.Combine(_options.TemplatesPath, templatePath);

            if (_options.UseTemplateCache && _templateCache.TryGetValue(fullPath, out var cachedTemplate))
            {
                return cachedTemplate;
            }

            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException($"Template file not found: {fullPath}");
            }

            var templateText = await File.ReadAllTextAsync(fullPath);
            var template = _handlebars.Compile(templateText);

            if (_options.UseTemplateCache)
            {
                _templateCache[fullPath] = template;
            }

            return template;
        }

        private object CreateTemplateData(TestStructure testStructure, TemplateOptions options)
        {
            return new
            {
                ns = options.Ns ?? _options.DefaultTemplateValues["Ns"],
                baseUrl = options.BaseUrl ?? _options.DefaultTemplateValues["BaseUrl"],
                pageName = testStructure.PageName,
                elements = testStructure.Elements,
                testCases = testStructure.TestCases,
                timestamp = DateTime.UtcNow,
                generator = "Playwright Test Generator"
            };
        }

        private void RegisterHelpers()
        {
            try
            {
                _handlebars.RegisterHelper("formatAction", (context, arguments) =>
                {
                    var action = arguments[0]?.ToString()?.ToLower();
                    return action switch
                    {
                        "click" => "ClickAsync",
                        "fill" => "FillAsync",
                        "check" => "CheckAsync",
                        "uncheck" => "UncheckAsync",
                        "press" => "PressAsync",
                        "type" => "TypeAsync",
                        _ => $"{action}Async"
                    };
                });

                _handlebars.RegisterHelper("formatAssertion", (context, arguments) =>
                {
                    var assertion = arguments[0]?.ToString()?.ToLower();
                    return assertion switch
                    {
                        "visible" => "ToBeVisibleAsync",
                        "hidden" => "ToBeHiddenAsync",
                        "enabled" => "ToBeEnabledAsync",
                        "disabled" => "ToBeDisabledAsync",
                        "contains" => "ToContainTextAsync",
                        _ => $"ToBe{assertion}Async"
                    };
                });

                _logger.LogInformation("Successfully registered Handlebars helpers");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to register Handlebars helpers");
                throw new TemplateConfigurationException("Failed to register template helpers", ex);
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _templateCache.Clear();
                _disposed = true;
            }
        }
    }
}