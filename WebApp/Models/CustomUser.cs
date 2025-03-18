using Microsoft.AspNetCore.Identity;

namespace WebApp.CustomIdentity
{
    public class CustomUser : IdentityUser
    {
        public string PasswordHash { get; set; }
    }
}