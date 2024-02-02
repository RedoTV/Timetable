using System.ComponentModel.DataAnnotations;

namespace TimetableServer.Models.DbModels;
public class Teacher
{
    [Key]
    public int Id { get; set; }
    public string FullName { get; set;} = null!;
}
