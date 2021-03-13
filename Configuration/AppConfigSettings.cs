using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Configuration
{
    public class AppConfigSettings : IApplicationSettings
    {
        private readonly IConfiguration _configuration;

        public AppConfigSettings()
        {
            _configuration = new ConfigurationBuilder().SetBasePath(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Configuration"))
                                                       .AddJsonFile("appconfig.json", false)
                                                       .Build();
        }

        public string ConnectionString
        {
            get { return _configuration?.GetConnectionString("DefaultConnection"); }
        }

        public string MailSettingsSmtpNetworkHost
        {
            get { return _configuration?["MailSettings:Smtp:Network:Host"]; }
        }

        public int MailSettingsSmtpNetworkPort
        {
            get { return int.Parse(_configuration?["MailSettings:Smtp:Network:Port"] ?? "0"); }
        }

        public string MailSettingsSmtpNetworkUserName
        {
            get { return _configuration?["MailSettings:Smtp:Network:UserName"]; }
        }

        public string MailSettingsSmtpNetworkPassword
        {
            get { return _configuration?["MailSettings:Smtp:Network:Password"]; }
        }

        public bool MailSettingsSmtpNetworkDefaultCredentials
        {
            get { return bool.Parse(_configuration?["MailSettings:Smtp:Network:DefaultCredentials"] ?? "false"); }
        }

        public int NumberOfResultsPerPage
        {
            get { return int.Parse(_configuration?["NumberOfResultsPerPage"]); }
        }
        
        public double CookieAuthenticationTimeout
        {
            get { return double.Parse(_configuration?["CookieAuthenticationTimeout"]); }
        }

        public string PayPalBusinessEmail
        {
            get { return _configuration?["PayPalBusinessEmail"]; }
        }

        public string PayPalPaymentPostToUrl
        {
            get { return _configuration?["PayPalPaymentPostToUrl"]; }
        }
    }
}