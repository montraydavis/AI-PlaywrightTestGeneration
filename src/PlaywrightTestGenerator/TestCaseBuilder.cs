using System;

namespace PlaywrightTestGenerator
{
    public class TestCaseBuilder
    {
        private readonly TestCase _testCase = new();

        public TestCaseBuilder WithName(string name)
        {
            _testCase.Name = name;
            return this;
        }

        public TestCaseBuilder WithDescription(string description)
        {
            _testCase.Description = description;
            return this;
        }

        public TestCaseBuilder AddTag(string tag)
        {
            _testCase.Tags.Add(tag);
            return this;
        }

        public TestCaseBuilder AddSetupStep(Action<TaskStepBuilder> configure)
        {
            var builder = new TaskStepBuilder();
            configure(builder);
            _testCase.Setup.Add(builder.Build());
            return this;
        }

        public TestCaseBuilder AddStep(Action<TaskStepBuilder> configure)
        {
            var builder = new TaskStepBuilder();
            configure(builder);
            _testCase.Steps.Add(builder.Build());
            return this;
        }

        public TestCaseBuilder AddAssertion(Action<TaskStepBuilder> configure)
        {
            var builder = new TaskStepBuilder();
            configure(builder);
            _testCase.Assertions.Add(builder.Build());
            return this;
        }

        public TestCaseBuilder AddCleanupStep(Action<TaskStepBuilder> configure)
        {
            var builder = new TaskStepBuilder();
            configure(builder);
            _testCase.Cleanup.Add(builder.Build());
            return this;
        }

        public TestCase Build() => _testCase;
    }
}
