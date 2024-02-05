namespace TimetableServer.Models.Requests;

public class GroupRequestForm
{
    public required string Name {get;set;} = null!;
    public required int SemesterId {get;set;}
}
