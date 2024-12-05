using System;

namespace PlaywrightTestGenerator.Exceptions
{
    public class TestGenerationException : Exception
    {
        public TestGenerationException(string message, Exception? innerException = null)
            : base(message, innerException) { }
    }
}
