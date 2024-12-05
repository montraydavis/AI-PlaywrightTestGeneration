using System;

namespace PlaywrightTestGenerator.Exceptions
{
    public class TemplateConfigurationException : Exception
    {
        public TemplateConfigurationException(string message, Exception innerException = null)
            : base(message, innerException) { }
    }
}
