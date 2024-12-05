using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PlaywrightTestGenerator
{
    public class PageElement
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("selector")]
        public string Selector { get; set; }
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("properties")]
        public Dictionary<string, string> Properties { get; set; } = new();
    }
}
