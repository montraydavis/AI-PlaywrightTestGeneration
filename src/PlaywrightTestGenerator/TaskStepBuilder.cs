namespace PlaywrightTestGenerator
{
    public class TaskStepBuilder
    {
        private readonly TaskStep _step = new();

        public TaskStepBuilder WithDescription(string description)
        {
            _step.Description = description;
            return this;
        }

        public TaskStepBuilder WithElement(string elementName)
        {
            _step.ElementName = elementName;
            return this;
        }

        public TaskStepBuilder WithAction(string action)
        {
            _step.Action = action;
            return this;
        }

        public TaskStepBuilder WithParameter(string key, string value)
        {
            _step.Parameters[key] = value;
            return this;
        }

        public TaskStep Build() => _step;
    }
}
