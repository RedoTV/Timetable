using System.ComponentModel.DataAnnotations;
using TimetableServer.HelperClasses;

namespace TimetableServer.Models.DbModels;
public class User
{
    [Key]
    public int id { get; set; }
    
    [Required]
    public string Name { get; set; } = null!;

    [Required]
    public string HashedPassword { get; set; } = null!;

    [Required]
    public string Role { get; set; } = RolesEnum.User;
}
