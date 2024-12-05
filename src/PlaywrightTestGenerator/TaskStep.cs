using System.Collections.Generic;

namespace PlaywrightTestGenerator
{
    public class TaskStep
    {
        public string Description { get; set; } = "(N/A)";
        public string ElementName { get; set; }
        public string Action { get; set; }
        public Dictionary<string, string> Parameters { get; set; } = [];
    }
}
