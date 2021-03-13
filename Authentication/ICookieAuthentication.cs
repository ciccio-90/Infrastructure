using System.Threading.Tasks;

namespace Infrastructure.Authentication
{
    public interface ICookieAuthentication
    {
        Task SetAuthenticationToken(string token);
        string GetAuthenticationToken();
        Task SignOut();
    }
}