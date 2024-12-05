using System.Collections.Generic;

namespace PlaywrightTestGenerator
{
    public class TestStructure
    {
        public string PageName { get; set; }
        public List<PageElement> Elements { get; set; } = new();
        public List<UserTask> Tasks { get; set; } = new();
        public List<TestCase> TestCases { get; set; } = new();
    }
}
