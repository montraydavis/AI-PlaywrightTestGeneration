using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PlaywrightTestGenerator
{
    public class UserTask
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("steps")]
        public List<TaskStep> Steps { get; set; } = new();
        [JsonPropertyName("prerequisites")]
        public List<string> Prerequisites { get; set; } = new();
        [JsonPropertyName("expectedResults")]
        public List<string> ExpectedResults { get; set; } = new();
    }
}
