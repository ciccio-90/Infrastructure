using System;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.CookieStorage
{
    public class CookieStorageService : ICookieStorageService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CookieStorageService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void Save(string key, string value, DateTime expires)
        {
            _httpContextAccessor?.HttpContext?.Response?.Cookies?.Append(key, value, new CookieOptions
            {
                Expires = expires,
                HttpOnly = true,
                Secure = _httpContextAccessor?.HttpContext?.Request?.IsHttps ?? false
            });
        }
        
        public string Retrieve(string key)
        {
            string cookie = _httpContextAccessor?.HttpContext?.Request?.Cookies?[key];
            
            if (cookie != null)
            {
                return cookie;
            }
            else
            {
                return string.Empty;
            }                           
        }
    }
}