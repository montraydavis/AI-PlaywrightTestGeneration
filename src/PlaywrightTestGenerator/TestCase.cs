using System.Collections.Generic;

namespace PlaywrightTestGenerator
{
    public class TestCase
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> Tags { get; set; } = new();
        public List<TaskStep> Setup { get; set; } = new();
        public List<TaskStep> Steps { get; set; } = new();
        public List<TaskStep> Assertions { get; set; } = new();
        public List<TaskStep> Cleanup { get; set; } = new();
    }
}
