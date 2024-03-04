using System.Net;
using System.Net.Mail;

namespace Codedberries.Services
{
    public class MailService
    {
        private string host = "";
        private int port = 587;
        private string username = "";
        private string password = "";

        public MailService(string host, int port, string username, string password)
        {
            this.host = host;
            this.port = port;
            this.username = username;
            this.password = password;
        }

        public bool SendMessage(string to, string subject, string body)
        {
            MailMessage message = new MailMessage(username, to);
            message.Subject = subject;
            message.Body = body;

            try
            {
                SmtpClient smtpClient = new SmtpClient()
                {
                    Host = host,
                    Port = port,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(username, password)
                };

                smtpClient.Send(message);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
