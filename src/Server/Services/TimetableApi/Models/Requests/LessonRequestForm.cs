namespace TimetableServer.Models.Requests;

public class LessonRequestForm
{
    public required string Teacher {get;set;} = null!;
    public required string Room {get;set;} = null!;
    public required string ClassName {get;set;} = null!;
    public required TimeSpan Start{get;set;}
    public required TimeSpan Finish{get;set;}
    public required int LessonNumber {get;set;}
    public required int DayNumber {get;set;}
}
