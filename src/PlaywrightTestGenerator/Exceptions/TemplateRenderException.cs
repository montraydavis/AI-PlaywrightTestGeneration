using System;

namespace PlaywrightTestGenerator.Exceptions
{
    public class TemplateRenderException : Exception
    {
        public TemplateRenderException(string message, Exception? innerException = null)
            : base(message, innerException) { }
    }
}
