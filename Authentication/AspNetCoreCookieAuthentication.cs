using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Authentication
{
    public class AspNetCoreCookieAuthentication : ICookieAuthentication
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AspNetCoreCookieAuthentication(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task SetAuthenticationToken(string token)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, token)
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await _httpContextAccessor?.HttpContext?.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
        }

        public string GetAuthenticationToken()
        {
            return _httpContextAccessor?.HttpContext?.User?.Identity?.Name;
        }

        public async Task SignOut()
        {
            await _httpContextAccessor?.HttpContext?.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}