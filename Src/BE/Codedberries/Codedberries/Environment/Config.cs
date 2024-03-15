namespace Codedberries.Environment
{
    public class Config
    {
        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpUsername { get; set; }
        public string SmtpPassword { get; set; }

        public string FrontendURL { get; set; }

        public string SecretKey { get; set; }
    }
}
