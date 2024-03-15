namespace Codedberries.Environment
{
    public class Config
    {
        public string SmtpHost { get; private set; }
        public int SmtpPort { get; private set; }
        public string SmtpUsername { get; private set; }
        public string SmtpPassword { get; private set; }

        public string FrontendURL { get; private set; }
    }
}
