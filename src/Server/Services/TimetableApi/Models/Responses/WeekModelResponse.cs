namespace TimetableServer.Models.Responses;

public class WeekModelResponse
{
    public ICollection<DayModelResponse> Days{ get; set;} = null!;
    public DateTime WeekStart{ get; set;}
    public DateTime WeekEnd{ get; set;}
}