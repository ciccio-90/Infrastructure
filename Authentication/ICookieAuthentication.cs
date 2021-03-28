using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Authentication
{
    public interface ICookieAuthentication
    {
        Task SetAuthenticationToken(string email, IEnumerable<string> roles);
        string GetAuthenticationToken();
        Task SignOut();
    }
}