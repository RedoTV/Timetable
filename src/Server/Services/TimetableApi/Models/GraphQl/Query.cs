using HotChocolate;
using TimetableServer.Models.DbModels;
using TimetableServer.Services.Interfaces;

namespace Server.Models.GraphQl;
public class Query
{
    public async Task<Week> GetWeek([Service(ServiceKind.Synchronized)] ILessonService lessonService, int groupId) => 
        await lessonService.GetCurrentWeekFullInfo(groupId);

    public async Task<IEnumerable<string>> GetFacultiesName([Service(ServiceKind.Synchronized)] ILessonService lessonService) => 
        await lessonService.GetFacultiesNameAsync();
} 