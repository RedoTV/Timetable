using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace TimetableServer.Models.DbModels;
public class Week
{
    [Key]
    public int Id {get;set;}
    public DateTime WeekStart {get;set;}
    public DateTime WeekEnd {get;set;}
    public int WeekNumber {get;set;}
    public bool IsOdd {get;set;} = true;
    public ICollection<Day> Days {get;set;} = null!;
    public int GroupId {get;set;}
    public Group Group {get;set;} = null!;
}