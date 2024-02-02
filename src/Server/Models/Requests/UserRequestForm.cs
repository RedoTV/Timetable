using System.ComponentModel.DataAnnotations;

namespace TimetableServer.Models.Requests;

public class UserRequestForm
{
    [Required]
    [MinLength(4)]
    public string Name { get; set; } = null!;

    [Required]
    [MinLength(8)]
    public string Password { get; set; } = null!;
}
