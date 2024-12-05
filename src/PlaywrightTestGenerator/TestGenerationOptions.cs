namespace PlaywrightTestGenerator
{
    public class TestGenerationOptions
    {
        public bool IncludeComments { get; set; } = true;
        public bool GenerateDataModels { get; set; } = true;
        public string TestFramework { get; set; } = "NUnit";
        public string OutputPath { get; set; }
    }
}
