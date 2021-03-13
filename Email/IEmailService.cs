namespace Infrastructure.Email
{
    public interface IEmailService
    {
        void SendMail(string from, string to, string subject, string body);
    }
}