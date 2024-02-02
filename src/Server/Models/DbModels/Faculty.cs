using System.ComponentModel.DataAnnotations;

namespace TimetableServer.Models.DbModels;

public class Faculty
{
    [Key]
    public int Id {get;set;}
    public string Name {get;set;} = null!;
    public ICollection<Semester> Semesters { get; set;} = new List<Semester>();
}