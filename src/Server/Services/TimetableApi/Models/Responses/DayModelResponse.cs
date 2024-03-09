namespace TimetableServer.Models.Responses;
public class DayModelResponse
{
    public int DayNumber { get; set; }
    public IEnumerable<LessonModelResponse> Lessons {get;set;} = null!;
}
