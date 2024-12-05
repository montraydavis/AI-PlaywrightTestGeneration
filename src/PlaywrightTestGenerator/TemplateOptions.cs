namespace PlaywrightTestGenerator
{
    public class TemplateOptions
    {
        public string Ns { get; set; } = "PlaywrightTests";
        public string BaseUrl { get; set; } = "http://localhost";
        public string TemplatePath { get; set; } = "Templates/CSTest.hbs";
    }
}