using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Authentication
{
    public class AspNetCoreIdentityAuthentication : ILocalAuthenticationService
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public AspNetCoreIdentityAuthentication(SignInManager<IdentityUser> signInManager,
                                                UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<User> Login(string email, string password)
        {
            User user = new User();
            user.IsAuthenticated = false;
            var result = await _signInManager.PasswordSignInAsync(email ?? string.Empty, password ?? string.Empty, false, true);

            if (result.Succeeded)
            {
                user.AuthenticationToken = Guid.NewGuid().ToString();
                user.Email = email;
                user.IsAuthenticated = true;
            }

            return user;
        }

        public async Task<User> RegisterUser(string email, string password)
        {
            User user = new User();
            user.IsAuthenticated = false;
            var identityUser = new IdentityUser { UserName = email, Email = email };
            var result = await _userManager.CreateAsync(identityUser, password);
                  
            if (result.Succeeded)
            {
                user.AuthenticationToken = Guid.NewGuid().ToString();
                user.Email = email;
                user.IsAuthenticated = true;
            }
            else
            {
                if (result.Errors?.Count() > 0)
                {
                    throw new InvalidOperationException(result.Errors?.FirstOrDefault()?.Description);
                }
                else
                {
                    throw new InvalidOperationException("There was a problem creating your account. Please try again.");
                }                
            }

            return user;
        }
    }
}