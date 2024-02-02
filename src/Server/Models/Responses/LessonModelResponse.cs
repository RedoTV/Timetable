namespace TimetableServer.Models.Responses;
public class LessonModelResponse
{
    public string Teacher { get; set;} = null!;

    public string Room { get;set;} = null!;

    public string ClassName { get; set;} = null!;

    public TimeSpan Start{ get; set;}

    public TimeSpan Finish{ get; set;}
}