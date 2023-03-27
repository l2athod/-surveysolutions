using System;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace WB.UI.Designer.CommonWeb
{
    public class MailSettings
    {
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string From { get; set; } = string.Empty;
        public bool EnableSSL { get; set; }
        public string PickupFolder { get; set; } = string.Empty;
        public bool UsePickupFolder { get; set; } = false;
    }

    public class MailSender : IEmailSender
    {
        private readonly IOptions<MailSettings> settings;
        private readonly IWebHostEnvironment env;

        public MailSender(IOptions<MailSettings> settings, IWebHostEnvironment env)
        {            
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
            this.env = env;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            if (string.IsNullOrWhiteSpace(settings.Value.Host))
            {
                return;
            }

            var config = this.settings.Value;

            var message = new MailMessage();
            message.From = new MailAddress(config.From, config.Username);
            message.To.Add(new MailAddress(email));
            message.Subject = subject;
            message.Body = htmlMessage;
            message.IsBodyHtml = true;




            var client = new SmtpClient();
            client.Host = config.Host;
            client.Port = config.Port;
            client.EnableSsl = config.EnableSSL;
            client.Credentials = new NetworkCredential(
            config.From,
            config.Password);

            await client.SendMailAsync(message);
        }
    }
}
