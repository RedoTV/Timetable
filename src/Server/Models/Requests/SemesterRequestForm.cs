using System.ComponentModel.DataAnnotations;

namespace TimetableServer.Models.Requests;

public class SemesterRequestForm
{
    [Required]
    public DateTime FromDate {get;set;}

    [Required]
    public DateTime ToDate {get;set;}
    
    [Required]
    public int FacultyId {get;set;} 
}
