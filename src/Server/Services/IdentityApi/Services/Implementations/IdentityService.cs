using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using IdentityApi.Data;
using IdentityApi.Models.DbModels;
using IdentityApi.Models.Requests;
using IdentityApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Extensions;

namespace IdentityApi.Services.Implementations
{
    public class IdentityService : IIdentityService
    {
        private readonly IConfiguration Configuration;
        private readonly IdentityDbContext IdentityDb;
        private readonly ILogger<IdentityService> Logger;
        public IdentityService(
            IConfiguration configuration, 
            IdentityDbContext identityDb, 
            ILogger<IdentityService> logger)
        {
            Configuration = configuration;
            IdentityDb = identityDb;
            Logger = logger;
        }

        public async Task<string> GetTokenAsync(User user)
        {
            return await Task.Run(() => {
                List<Claim> claimsForToken = new List<Claim>{
                        new Claim(ClaimTypes.Name, user.Name),
                        new Claim("Role", user.Role.GetDisplayName())
                    };

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetSection("Jwt:Key").Value!));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

                var token = new JwtSecurityToken(
                    claims: claimsForToken,
                    issuer: Configuration.GetSection("Jwt:Issuer").Value,
                    audience: Configuration.GetSection("Jwt:Audience").Value,
                    signingCredentials: credentials,
                    expires: DateTime.Now.AddHours(6)
                );
                string jwt = new JwtSecurityTokenHandler().WriteToken(token);
                return jwt;
            });
        }

        public async Task<User?> GetUserAsync(UserFormInfo userForm)
        {
            User? user = null;
            User userBuilder = new User
            {
                Name = userForm.Name,
                HashedPassword = await HashPasswordAsync(userForm.Password)
            };
            
            try
            {
                user = await IdentityDb.Users
                    .Where(u => u.Name == userBuilder.Name && u.HashedPassword == userBuilder.HashedPassword)
                    .FirstAsync();
            }
            catch (Exception)
            {
                Logger.LogWarning("User not found");
                return null;
            }
            return user;
        }

        public async Task<string?> RegisterUserAsync(UserFormInfo userFormInfo)
        {
            if(userFormInfo.Name == null && userFormInfo.Password == null)
            {
                Logger.LogError("user name or password is empty");
                return null;
            }

            User? user = new User
            {
                Name = userFormInfo.Name!,
                HashedPassword = await HashPasswordAsync(userFormInfo.Password)
            };

            await IdentityDb.Users.AddAsync(user);
            await IdentityDb.SaveChangesAsync();

            User userInDb = await IdentityDb.Users
                .Where(u=>u.Name == user.Name && u.HashedPassword == user.HashedPassword)
                .FirstAsync();

            return await GetTokenAsync(userInDb);
        }

        private async Task<string> HashPasswordAsync(string password)
        {
            return await Task.Run(()=> {
                SHA256 sha = SHA256.Create();
                byte[] passwordInByte = Encoding.Unicode.GetBytes(password);
                byte[] hashedPassword = sha.ComputeHash(passwordInByte);
                return Convert.ToBase64String(hashedPassword);
            });
            
        }
    }
}