using System.Collections.Generic;

namespace Infrastructure.Authentication
{
    public class User
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public bool IsAuthenticated { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}