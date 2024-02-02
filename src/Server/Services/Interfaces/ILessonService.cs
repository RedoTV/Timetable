using TimetableServer.Models.DbModels;

namespace TimetableServer.Services.Interfaces;

public interface ILessonService
{
    //build all tables
    public Task<bool> AddFaculties(ICollection<Faculty> faculties);
    public Task<bool> AddSemester(Semester semester);
    public Task<bool> AddGroups(ICollection<Group> groups);
    public Task<bool> InitializeSemesterWeeks(int semesterId, bool firstWeekIsOdd);
    public Task<bool> InitializeWeekDays(int semesterId);
    public Task<bool> AddLessons(ICollection<Lesson> lessons, bool firstWeekIsOdd, int group);
    public Task<bool> AddTeachers(ICollection<Teacher> teacher);

    //get info from db
    public Task GetFacultiesName();
    public Task GetSemesters();
    public Task GetWeekById(int weekId);
    public Task GetCurrentWeek();
}
