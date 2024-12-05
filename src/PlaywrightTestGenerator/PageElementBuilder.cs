namespace PlaywrightTestGenerator
{
    public class PageElementBuilder
    {
        private readonly PageElement _element = new();

        public PageElementBuilder WithName(string name)
        {
            _element.Name = name;
            return this;
        }

        public PageElementBuilder WithSelector(string selector)
        {
            _element.Selector = selector;
            return this;
        }

        public PageElementBuilder WithType(string type)
        {
            _element.Type = type;
            return this;
        }

        public PageElementBuilder WithDescription(string description)
        {
            _element.Description = description;
            return this;
        }

        public PageElementBuilder WithProperty(string key, string value)
        {
            _element.Properties[key] = value;
            return this;
        }

        public PageElement Build() => _element;
    }
}
