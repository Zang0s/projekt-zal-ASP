using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace WebApp.CustomIdentity
{
    public class CustomUserStore : IUserStore<CustomUser>, IUserPasswordStore<CustomUser>
    {
        private readonly List<CustomUser> _users;

        public CustomUserStore()
        {
            var json = System.IO.File.ReadAllText("wwwroot/users.json");
            _users = JsonConvert.DeserializeObject<List<CustomUser>>(json) ?? new List<CustomUser>();
        }

        public Task<CustomUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            var user = _users.FirstOrDefault(u => u.UserName.ToUpper() == normalizedUserName);
            return Task.FromResult(user);
        }

        public Task<string> GetPasswordHashAsync(CustomUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(CustomUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }
        
        public void Dispose() { }
        public Task<IdentityResult> CreateAsync(CustomUser user, CancellationToken cancellationToken) => Task.FromResult(IdentityResult.Success);
        public Task<IdentityResult> DeleteAsync(CustomUser user, CancellationToken cancellationToken) => Task.FromResult(IdentityResult.Success);
        public Task<IdentityResult> UpdateAsync(CustomUser user, CancellationToken cancellationToken) => Task.FromResult(IdentityResult.Success);
        public Task<CustomUser> FindByIdAsync(string userId, CancellationToken cancellationToken) => Task.FromResult(_users.FirstOrDefault(u => u.Id == userId));
        public Task<string> GetUserIdAsync(CustomUser user, CancellationToken cancellationToken) => Task.FromResult(user.Id);
        public Task<string> GetUserNameAsync(CustomUser user, CancellationToken cancellationToken) => Task.FromResult(user.UserName);
        public Task SetUserNameAsync(CustomUser user, string userName, CancellationToken cancellationToken) => Task.CompletedTask;
        public Task SetPasswordHashAsync(CustomUser user, string passwordHash, CancellationToken cancellationToken) => Task.CompletedTask;
        public Task<string> GetNormalizedUserNameAsync(CustomUser user, CancellationToken cancellationToken) => Task.FromResult(user.UserName.ToUpper());
        public Task SetNormalizedUserNameAsync(CustomUser user, string normalizedName, CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
