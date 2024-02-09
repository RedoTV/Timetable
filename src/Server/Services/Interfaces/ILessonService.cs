using TimetableServer.Models.DbModels;
using TimetableServer.Models.Requests;

namespace TimetableServer.Services.Interfaces;

public interface ILessonService
{
    //build all tables
    public Task<bool> AddFaculties(string[] facultiesName);
    public Task<bool> AddSemester(SemesterRequestForm semester);
    public Task<bool> AddGroups(GroupRequestForm[] groups);
    public Task<bool> InitializeSemesterWeeks(int semesterId, bool firstWeekIsOdd);
    public Task<bool> InitializeWeekDays(int semesterId);
    public Task<bool> AddLessonsToGroup(ICollection<LessonRequestForm> lessons, bool firstWeekIsOdd, int groupId);
    public Task<bool> AddTeachers(ICollection<Teacher> teacher);

    //get info from db
    public Task<IEnumerable<string>> GetFacultiesNameAsync();
    public Task<IEnumerable<Semester>> GetSemestersAsync(int facultyId);
    public Task GetWeekById(int weekId);
    public Task<Week> GetCurrentWeekAsync(int groupId);
    public Task<Day> GetCurrentDayAsync(int groupId);
    public Task<Week> GetCurrentWeekFullInfo(int groupId);
}
