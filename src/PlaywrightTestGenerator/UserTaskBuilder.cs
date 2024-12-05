using System;

namespace PlaywrightTestGenerator
{
    public class UserTaskBuilder
    {
        private readonly UserTask _task = new();

        public UserTaskBuilder WithName(string name)
        {
            _task.Name = name;
            return this;
        }

        public UserTaskBuilder WithDescription(string description)
        {
            _task.Description = description;
            return this;
        }

        public UserTaskBuilder AddStep(Action<TaskStepBuilder> configure)
        {
            var builder = new TaskStepBuilder();
            configure(builder);
            _task.Steps.Add(builder.Build());
            return this;
        }

        public UserTaskBuilder AddPrerequisite(string prerequisite)
        {
            _task.Prerequisites.Add(prerequisite);
            return this;
        }

        public UserTaskBuilder AddExpectedResult(string result)
        {
            _task.ExpectedResults.Add(result);
            return this;
        }

        public UserTask Build() => _task;
    }
}
