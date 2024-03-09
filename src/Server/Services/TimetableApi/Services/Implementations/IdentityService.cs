using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TimetableServer.Database;
using TimetableServer.HelperClasses;
using TimetableServer.Models.DbModels;
using TimetableServer.Models.Requests;
using TimetableServer.Models.Responses;
using TimetableServer.Services.Interfaces;

namespace TimetableServer.Services.Implementations;

public class IdentityService : IIdentityService
{
    private TimetableDBContext TimetableDB {get; set;}
    public IdentityService(TimetableDBContext timetableDB)
    {
        TimetableDB = timetableDB;
    }

    public async Task<LoginResponse> LoginUser(UserRequestForm userRequest)
    {
        string hashedPassword = await HashPassword(userRequest.Password);
        User user = null!;
        try
        {
            user = TimetableDB.Users
                .Where(u => u.Name == userRequest.Name && u.HashedPassword == hashedPassword)
                .First();
        }
        catch
        {
            return null!;
        }

        string role = TimetableDB.Users
            .Where(u => u.Name == userRequest.Name && u.HashedPassword == hashedPassword)
            .First().Role;

        var claims = new List<Claim> 
        {  
            new Claim(ClaimTypes.Name, userRequest.Name), 
            new Claim(ClaimTypes.Role, role)
        };

        var jwt = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                claims: claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromDays(1)), // время действия 1 день
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

        string jwtToken = new JwtSecurityTokenHandler().WriteToken(jwt);
        LoginResponse response = new LoginResponse(){ Token = jwtToken, Role = role};
        return await Task.FromResult(response);
    }

    public async Task<LoginResponse> RegisterUser(UserRequestForm userRequest)
    {
        string hashedPassword = await HashPassword(userRequest.Password);
        User user = new User()
        { 
            Name = userRequest.Name,
            HashedPassword = hashedPassword 
        };
        await TimetableDB.Users.AddAsync(user);
        await TimetableDB.SaveChangesAsync();
        
        var claims = new List<Claim> 
        {  
            new Claim(ClaimTypes.Name, user.Name), 
            new Claim(ClaimTypes.Role, user.Role)
        };

        var jwt = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                claims: claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromDays(1)), // время действия 1 день
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

        string jwtToken = new JwtSecurityTokenHandler().WriteToken(jwt);

        LoginResponse response = new LoginResponse(){ Token = jwtToken, Role = user.Role};
        return response;
    }

    private async Task<string> HashPassword(string password)
    {
        SHA256 sha = SHA256.Create();
        byte[] passwordInByte = Encoding.Unicode.GetBytes(password);
        byte[] hashedPassword = sha.ComputeHash(passwordInByte);
        return await Task.FromResult(Convert.ToBase64String(hashedPassword));
    }
}
