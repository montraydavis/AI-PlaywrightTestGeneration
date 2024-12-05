using System.Collections.Generic;

namespace PlaywrightTestGenerator
{
    public class TestTemplateOptions
    {
        public string TemplatesPath { get; set; } = "Templates";
        public bool UseTemplateCache { get; set; } = true;
        public Dictionary<string, string> DefaultTemplateValues { get; set; } = new()
        {
            { "Ns", "PlaywrightTests" },
            { "BaseUrl", "http://localhost" }
        };
    }
}