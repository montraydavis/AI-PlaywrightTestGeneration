using System;

namespace PlaywrightTestGenerator
{
    public class TestGenerationRequestBuilder
    {
        private readonly TestGenerationRequest _request = new();

        public TestGenerationRequestBuilder WithPageDescription(string description)
        {
            _request.PageDescription = description;
            return this;
        }

        public TestGenerationRequestBuilder WithContext(string key, string value)
        {
            _request.AdditionalContext[key] = value;
            return this;
        }

        public TestGenerationRequestBuilder WithOptions(Action<TestGenerationOptionsBuilder> configure)
        {
            var builder = new TestGenerationOptionsBuilder();
            configure(builder);
            _request.Options = builder.Build();
            return this;
        }

        public TestGenerationRequest Build() => _request;
    }
}
