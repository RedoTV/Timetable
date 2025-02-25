using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TimetableServer.Models.DbModels;
public class Lesson
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id{get;set;}

    [Required]
    public Teacher Teacher {get;set;} = null!;

    [Required]
    public string Room {get;set;} = null!;

    [Required]
    public string ClassName {get;set;} = null!;

    [Required]
    public TimeSpan Start{get;set;}

    [Required]
    public TimeSpan Finish{get;set;}
    
    [Required]
    public int LessonNumber {get;set;}

    public int DayId {get;set;}
    public Day Day { get; set; } = null!;

    public Lesson ShallowCopy()
    {
        return (Lesson) this.MemberwiseClone();
    }
}