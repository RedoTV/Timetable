using System.ComponentModel.DataAnnotations;

namespace TimetableServer.Models.DbModels;
public class Teacher
{
    [Key]
    public int Id { get; set; }
    public string FirstName { get; set;} = null!;
    public string LastName { get; set;} = null!;
    public string MiddleName { get; set;} = null!;
}
