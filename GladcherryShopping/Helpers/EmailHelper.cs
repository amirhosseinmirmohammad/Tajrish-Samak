using System.Net;
using System.Net.Mail;

namespace GladCherryShopping.Helpers
{
    public class EmailHelper
    {
        private readonly string _smtpServer;
        private readonly string _fromEmail;
        private readonly string _displayName;
        private readonly int _portNumber;

        public EmailHelper(int portNumber, string smtpServer, string fromEmail,string displayName)
        {
            _smtpServer = smtpServer;
            _fromEmail = fromEmail;
            _displayName = displayName;
            _portNumber = portNumber;
        }

        public void Send(string userName, string password, string toEmail, string subject, string body, bool isHtml)
        {
            var sendFrom = new MailAddress(_fromEmail,_displayName);
            var sendTo = new MailAddress(toEmail);
            var message = new MailMessage(sendFrom,sendTo) { Body = body, Subject = subject, IsBodyHtml = isHtml};
            var emailClient = new SmtpClient(_smtpServer, _portNumber)
            {
                Credentials = new NetworkCredential(userName, password),
                EnableSsl = false
            };
            emailClient.Send(message);
        }
    }
}