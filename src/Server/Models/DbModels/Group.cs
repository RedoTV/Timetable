using System.ComponentModel.DataAnnotations;

namespace TimetableServer.Models.DbModels;

public class Group
{
    [Key]
    public int Id {get;set;}
    public string Name {get;set;} = null!;
    public ICollection<Week?> Weeks {get;set;} = new List<Week?>();
    public int SemesterId {get;set;}
    public Semester Semester {get;set;} = null!;
}