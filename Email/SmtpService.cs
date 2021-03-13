using System.Net;
using System.Net.Mail;
using Infrastructure.Configuration;

namespace Infrastructure.Email
{
    public class SmtpService : IEmailService
    {
        private readonly IApplicationSettings _applicationSettings;

        public SmtpService(IApplicationSettings applicationSettings)
        {
            _applicationSettings = applicationSettings;
        }

        public void SendMail(string from, string to, string subject, string body)
        {
            using (MailMessage message = new MailMessage())
            {
                message.From = new MailAddress(from);
                
                message.To.Add(to);
                
                message.Subject = subject;
                message.Body = body;
                
                using (SmtpClient smtp = new SmtpClient(_applicationSettings.MailSettingsSmtpNetworkHost, _applicationSettings.MailSettingsSmtpNetworkPort))
                {
                    smtp.UseDefaultCredentials = _applicationSettings.MailSettingsSmtpNetworkDefaultCredentials;
                    smtp.Credentials = new NetworkCredential(_applicationSettings.MailSettingsSmtpNetworkUserName, _applicationSettings.MailSettingsSmtpNetworkPassword);
                    
                    smtp.Send(message);
                }      
            }
        }
    }
}