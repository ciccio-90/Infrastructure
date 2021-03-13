using System.Threading.Tasks;

namespace Infrastructure.Authentication
{
    public interface ILocalAuthenticationService
    {
        Task<User> Login(string email, string password);
        Task<User> RegisterUser(string email, string password);
    }
}