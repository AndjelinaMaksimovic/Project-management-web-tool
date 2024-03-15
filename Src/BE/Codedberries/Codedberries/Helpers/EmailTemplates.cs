namespace Codedberries.Helpers
{
    public static class EmailTemplates
    {
        public static string ActivationEmail(string firstname, string lastname, string link)
        {
            return
                $"Dear <b>{firstname} {lastname}</b>,<br><br>" +
                "Congratulations! Your registration was successful.<br><br>" +
                $"Please click on <a href='{link}'>link</a> to activate your account.<br><br>" +
                "Once activated, you will be prompted to set your password.<br>" +
                "If you have any questions or need assistance, feel free to contact our support team at [codedberries.pm@gmail.com].<br><br>" +
                "Best regards,<br><br>CodedBerries team<br>Customer Support Team";
        }
    }
}
