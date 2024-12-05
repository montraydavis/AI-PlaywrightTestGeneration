using System;

namespace PlaywrightTestGenerator
{
    public class TestStructureBuilder
    {
        private readonly TestStructure _structure = new();

        public TestStructureBuilder WithPageName(string name)
        {
            _structure.PageName = name;
            return this;
        }

        public TestStructureBuilder AddElement(Action<PageElementBuilder> configure)
        {
            var builder = new PageElementBuilder();
            configure(builder);
            _structure.Elements.Add(builder.Build());
            return this;
        }

        public TestStructureBuilder AddTask(Action<UserTaskBuilder> configure)
        {
            var builder = new UserTaskBuilder();
            configure(builder);
            _structure.Tasks.Add(builder.Build());
            return this;
        }

        public TestStructureBuilder AddTestCase(Action<TestCaseBuilder> configure)
        {
            var builder = new TestCaseBuilder();
            configure(builder);
            _structure.TestCases.Add(builder.Build());
            return this;
        }

        public TestStructure Build() => _structure;
    }
}
