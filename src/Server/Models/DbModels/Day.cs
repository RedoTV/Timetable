using System.ComponentModel.DataAnnotations;

namespace TimetableServer.Models.DbModels;
public class Day
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int DayNumber { get; set; }
    public DateTime Date {get;set;}
    public ICollection<Lesson> Lessons {get;set;} = new List<Lesson>();
    public int WeekId {get;set;}
    public Week Week {get;set;} = null!;
}