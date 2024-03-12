namespace Codedberries.Helpers
{
    public static class EmailTemplates
    {
        public static string ActivationEmail(string firstname, string lastname, string link)
        {
            return
                $"Dear {firstname} {lastname},\n\n" +
                "Congratulations! Your registration was successful. Please click on the following link to activate your account:\n\n" +
                $"{link}\n\n" +
                "Once activated, you will be prompted to set your password.\n\n" +
                "If you have any questions or need assistance, feel free to contact our support team at [codedberries.pm@gmail.com].\n\n" +
                "Best regards,\n\n[Company name]\nCustomer Support Team";
        }
    }
}
