using System.Net.Mail;

namespace Codedberries.Helpers
{
    public static class Helper
    {
        public static bool IsEmailValid(string email)
        {
            try
            {
                MailAddress m = new MailAddress(email);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
