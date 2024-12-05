using System;

namespace PlaywrightTestGenerator.Exceptions
{
    public class TemplateNotFoundException : Exception
    {
        public TemplateNotFoundException(string message, Exception? innerException = null)
            : base(message, innerException) { }
    }
}
