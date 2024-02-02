namespace TimetableServer.Models.Requests;

public class GroupRequestForm
{
    public string Name {get;set;} = null!;
    public int SemesterId {get;set;}
}
