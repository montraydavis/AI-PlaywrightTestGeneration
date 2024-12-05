namespace PlaywrightTestGenerator
{
    public class TestGenerationOptionsBuilder
    {
        private readonly TestGenerationOptions _options = new();

        public TestGenerationOptionsBuilder WithComments(bool include = true)
        {
            _options.IncludeComments = include;
            return this;
        }

        public TestGenerationOptionsBuilder WithDataModels(bool generate = true)
        {
            _options.GenerateDataModels = generate;
            return this;
        }

        public TestGenerationOptionsBuilder WithTestFramework(string framework)
        {
            _options.TestFramework = framework;
            return this;
        }

        public TestGenerationOptionsBuilder WithOutputPath(string path)
        {
            _options.OutputPath = path;
            return this;
        }

        public TestGenerationOptions Build() => _options;
    }
}
