namespace Infrastructure.Configuration
{
    public interface IApplicationSettings
    {
        string ConnectionString { get; }
        string MailSettingsSmtpNetworkHost { get; }
        int MailSettingsSmtpNetworkPort { get; }
        string MailSettingsSmtpNetworkUserName { get; }
        string MailSettingsSmtpNetworkPassword { get; }
        bool MailSettingsSmtpNetworkDefaultCredentials { get; }
        int NumberOfResultsPerPage { get; }
        double CookieAuthenticationTimeout { get; }
        string PayPalBusinessEmail { get; }
        string PayPalPaymentPostToUrl { get; }
    }
}