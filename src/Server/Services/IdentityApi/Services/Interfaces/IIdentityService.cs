using IdentityApi.Models.DbModels;
using IdentityApi.Models.Requests;

namespace IdentityApi.Services.Interfaces
{
    public interface IIdentityService
    {
        public Task<string> GetTokenAsync(User user);
        public Task<User?> GetUserAsync(UserFormInfo userForm);
        public Task<string?> RegisterUserAsync(UserFormInfo userFormInfo);
    }
}