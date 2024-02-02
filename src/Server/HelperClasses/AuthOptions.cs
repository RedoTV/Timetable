using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace TimetableServer.HelperClasses;

public class AuthOptions
{
    public const string ISSUER = "TimetableServer"; // издатель токена
    public const string AUDIENCE = "TimetableClients"; // потребитель токена
    const string KEY = "D32As34Je10O-221kO21Op1V!nm2";   // ключ для шифрации
    public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
}
