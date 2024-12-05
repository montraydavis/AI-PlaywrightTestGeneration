using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PlaywrightTestGenerator
{
    public class TestGenerationRequest
    {
        [JsonPropertyName("pageDescription")]
        public string PageDescription { get; set; }
        [JsonPropertyName("additionalContext")]
        public Dictionary<string, string> AdditionalContext { get; set; } = new();
        [JsonPropertyName("options")]
        public TestGenerationOptions Options { get; set; } = new();
    }
}
