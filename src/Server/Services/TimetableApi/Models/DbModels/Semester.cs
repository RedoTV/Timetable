using System.ComponentModel.DataAnnotations;

namespace TimetableServer.Models.DbModels;

public class Semester
{
    [Key]
    public int Id {get;set;}
    public DateTime FromDate {get;set;}
    public DateTime ToDate {get;set;}
    public ICollection<Group> Groups {get;set;} = new List<Group>();
    public int FacultyId {get;set;}
    public Faculty Faculty {get;set;} = null!;
}