using System.ComponentModel.DataAnnotations;

namespace TimetableServer.Models.Responses;
public class LoginResponse
{
    [Required]
    public string Token { get; set; } = null!;

    [Required]
    public string Role { get; set; } = null!;
}
