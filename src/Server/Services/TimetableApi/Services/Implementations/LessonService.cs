using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TimetableServer.Database;
using TimetableServer.HelperClasses;
using TimetableServer.Models.DbModels;
using TimetableServer.Models.Requests;
using TimetableServer.Services.Interfaces;

namespace TimetableServer.Services.Implementations;

public class LessonService : ILessonService
{
    private TimetableDBContext TimetableDB {get;set;}
    private ILogger _logger {get;set;}
    public LessonService(TimetableDBContext timetableDB, ILogger<LessonService> logger)
    {
        TimetableDB = timetableDB;
        _logger = logger;
    }

    public async Task<bool> AddFaculties(string[] facultiesName)
    {
        _logger.LogInformation("Adding Faculties ...");
        try
        {
            await TimetableDB.Faculties.AddRangeAsync(facultiesName.Select(f => new Faculty(){ Name = f }));
            await TimetableDB.SaveChangesAsync();
            _logger.LogInformation("faculties added");
            return true;
        }
        catch (Exception exc)
        {
            _logger.LogCritical("Faculties wasn't added: " + exc.Message);
            return false;
        }
    }

    public async Task<bool> AddSemester(SemesterRequestForm semesterForm)
    {
        _logger.LogInformation("Adding Semester...");
        try
        {
            Semester semester = new Semester(){
                FromDate = semesterForm.FromDate,
                ToDate = semesterForm.ToDate,
                FacultyId = semesterForm.FacultyId
            };

            TimetableDB.Faculties
                .Where(f => f.Id == semesterForm.FacultyId)
                .FirstOrDefault()!
                .Semesters.Add(semester);

            await TimetableDB.SaveChangesAsync();
            _logger.LogInformation("Semester added");
            return true;
        }
        catch (Exception exc)
        {
            _logger.LogCritical("Semester wasn't added: " + exc.Message);
            return false;
        }
    }

    public async Task<bool> AddGroups(GroupRequestForm[] groupsForm)
    {
        try
        {
            if(groupsForm.Count() == 0)
                return false;

            _logger.LogInformation("Take semesterId from all groups");
            int semesterId = groupsForm.Where(gr => gr.SemesterId != 0).First()!.SemesterId;

            _logger.LogInformation("Find semester by semesterId");
            Semester semester = TimetableDB.Semesters.Where(s => s.Id == semesterId).First();

            _logger.LogInformation("Adding Groups...");
            ICollection<Group> groups = new List<Group>();
            foreach (var group in groupsForm)
            {
                groups.Add(new Group(){
                    Name = group.Name,
                    SemesterId = group.SemesterId
                });
            }

            foreach (Group group in groups)
            {
                group.SemesterId = semesterId;
                group.Semester = semester;
                semester.Groups.Add(group);
            }

            await TimetableDB.SaveChangesAsync();
            _logger.LogInformation("Groups added");
            return true;
        }
        catch (Exception exc)
        {
            _logger.LogCritical("Groups weren't added: " + exc.Message);
            return false;
        }
    }

    public async Task<bool> InitializeSemesterWeeks(int semesterId,bool firstWeekIsOdd)
    {
        try{
            _logger.LogInformation("Finding semester by semesterID");
            Semester? semester = await TimetableDB.Semesters.FindAsync(semesterId);

            _logger.LogInformation("getting all groups by semesterID");
            ICollection<Group> groups = await TimetableDB.Groups
                .Where(gr => gr.SemesterId == semesterId).ToListAsync();
            
            _logger.LogInformation("Initializing all weeks...");
            if((semester == null) || (groups == null))
                throw new NullReferenceException();
            
            await AddWeeksForAllGroups(firstWeekIsOdd,semester,groups);

            _logger.LogInformation("Weeks are initialized");
        }
        catch(Exception exc){
            _logger.LogCritical("Weeks aren't initialized: " + exc.Message);
            return false;
        }
        return true;  
    }

    private async Task AddWeeksForAllGroups(bool firstWeekIsOdd, Semester semester, ICollection<Group> groups)
    {
        DateTime dateOfStart;
        DateTime dateOfEnd;
        foreach (Group group in groups)
        {
            dateOfStart = semester.FromDate;
            dateOfEnd = semester.ToDate;
            for(int i = 1; dateOfStart != dateOfEnd; i++)
            {
                if(i != 1)
                {
                    //for record like: Monday - Sunday, in table
                    dateOfStart = dateOfStart.AddDays(1);
                }

                await TimetableDB.Weeks.AddAsync(new Week(){
                    WeekNumber = i,
                    WeekStart = dateOfStart,
                    WeekEnd = GetCurrentWeekEnd(dateOfStart,dateOfEnd),
                    GroupId = group.Id,
                    IsOdd = CurrentWeekIsOdd(i, firstWeekIsOdd)    
                });

                if(CurrentWeekIsLast(dateOfStart,dateOfEnd) == true)
                {
                    break;
                }
                else
                {
                    int numberOfDays = 7 - Convert.ToInt32(dateOfStart.DayOfWeek);
                    dateOfStart = dateOfStart.AddDays(numberOfDays);
                }
            }
        }
        await TimetableDB.SaveChangesAsync();
    }

    private bool CurrentWeekIsOdd(int i,bool firstWeekIsOdd)
    {
        return firstWeekIsOdd ? 
            (i % 2 != 0 ? true : false) :
            (i % 2 != 0 ? false : true);
    }

    private DateTime GetCurrentWeekEnd(DateTime dateOfStart,DateTime dateOfEnd)
    {
        if(dateOfStart.AddDays(7) > dateOfEnd)
        {
            return dateOfEnd;
        }
        else
        {
            int numberOfDays = 7 - Convert.ToInt32(dateOfStart.DayOfWeek);
            return dateOfStart.AddDays(numberOfDays);
        }
    }

    private bool CurrentWeekIsLast(DateTime dateOfStart,DateTime dateOfEnd)
    {
        return (dateOfStart.AddDays(7) > dateOfEnd) ? true : false;
    }

    //use only one time when add a semeter
    public async Task<bool> InitializeWeekDays(int semesterId)
    {
        try{
            _logger.LogInformation("Finding semester by semesterID");
            Semester semester = (await TimetableDB.Semesters.FindAsync(semesterId))!;

            _logger.LogInformation("getting all groups by semesterID");
            ICollection<Group> groups = TimetableDB.Groups
                .Where(gr => gr.SemesterId == semesterId)!.ToList();
            _logger.LogInformation($"count of received groups {groups.Count()}");

            if(groups == null) 
                return false;

            _logger.LogInformation($"initializing 6 days in every week in chosen semester, semesterId:{semesterId}");
            ICollection<Week> weeks;
            int i;
            foreach (var group in groups)
            {
                weeks = TimetableDB.Weeks.Where(w => w.GroupId == group.Id).ToList();
                foreach (var week in weeks)
                {
                    i = 0;
                    while(week.WeekStart.AddDays(i) < week.WeekEnd)
                    {
                        await TimetableDB.Days.AddAsync(new Day(){ 
                            DayNumber = (int)week.WeekStart.DayOfWeek + i,
                            Week = week!,
                            WeekId = week!.Id,
                            Date = week.WeekStart.AddDays(i)
                        });
                        i++;
                    }
                }
            }
            await TimetableDB.SaveChangesAsync();       

            _logger.LogInformation("Days are initialized");
        }
        catch(Exception exc){
            _logger.LogCritical("Days aren't initialized: " + exc.Message);
            return false;
        }
        return true;
    }

    public async Task<bool> AddLessonsToGroup(ICollection<LessonRequestForm> lessons, bool firstWeekIsOdd, int groupId)
    {
        try
        {
            _logger.LogInformation("Finding weeks by groupId");
            IEnumerable<Week> weeks = await TimetableDB.Weeks.Where((week) => week.GroupId == groupId).ToListAsync();
            IEnumerable<int> weeksId;
            
            _logger.LogInformation("Choosing: odd or even weeks");
            if(firstWeekIsOdd == true)
                weeksId = weeks.Where((week,index) => (index % 2) == 0).Select(w => w.Id);
            else
                weeksId = weeks.Where((week,index) => (index % 2) != 0).Select(w => w.Id);

            _logger.LogInformation("Adding lessons to every chosen week");
            await AddLessonsToChosenDaysAsync(lessons,weeksId);

            await TimetableDB.SaveChangesAsync();
            return true;
        }
        catch(Exception exc){
            _logger.LogCritical("Lessons aren't added: " + exc.Message);
            return false;
        }
    }

    private async Task<Teacher> GetTeacherAsync(string fullName)
    {     
        string[] splitedFullName = fullName.Trim().Split(' ',StringSplitOptions.RemoveEmptyEntries);
        
        return (await TimetableDB.Teachers.Where(t => 
            t.FirstName == splitedFullName[0] &&
            t.LastName == splitedFullName[1] &&
            t.MiddleName == splitedFullName[2]
        ).FirstOrDefaultAsync())!;
    }

    private async Task AddLessonsToChosenDaysAsync(ICollection<LessonRequestForm> lessons, IEnumerable<int> weeksId)
    {    
        var days =  from day in TimetableDB.Days
                    from week in TimetableDB.Weeks
                    where weeksId.Contains(week.Id)
                    where week.Days.Contains(day)
                    select new
                    {
                        DayId = day.Id,
                        DayNumber = day.DayNumber
                    };

        ICollection<Lesson> addedLessons = new List<Lesson>();
        Lesson lessonTemp;
        foreach (var lesson in lessons)
        {
            lessonTemp = lesson.ParseToDbModel(await GetTeacherAsync(lesson.Teacher));
            foreach(var day in days)
            {
                if(lesson.DayNumber == day.DayNumber)
                {
                    lessonTemp.DayId = day.DayId;
                    addedLessons.Add(lessonTemp.ShallowCopy());
                }
            }
        }

        await TimetableDB.AddRangeAsync(addedLessons);
        await TimetableDB.SaveChangesAsync();
    }

    public async Task<bool> AddTeachers(ICollection<Teacher> teacher)
    {
        try{
            if(teacher.Count() == 0)
                return false;

            _logger.LogInformation("Adding teachers to database");
            await TimetableDB.Teachers.AddRangeAsync(teacher);

            await TimetableDB.SaveChangesAsync();    
            _logger.LogInformation("Teachers were added");
        }
        catch(Exception exc){
            _logger.LogCritical("Teachers weren't added: " + exc.Message);
            return false;
        }
        return true;
    }

    public async Task<IEnumerable<string>> GetFacultiesNameAsync()
    {
        return await TimetableDB.Faculties.Select(f => f.Name).ToArrayAsync();
    }

    public async Task<IEnumerable<Semester>> GetSemestersAsync(int facultyId)
    {
        return await TimetableDB.Semesters.Where(s => s.FacultyId == facultyId).ToArrayAsync();
    }

    public Task GetWeekById(int weekId)
    {
        throw new NotImplementedException();
    }

    public async Task<Week> GetCurrentWeekAsync(int groupId)
    {
        return (await TimetableDB.Weeks
            .Where(w => w.GroupId == groupId)
            .Where(w => w.WeekStart < DateTime.Now && DateTime.Now < w.WeekEnd)
            .FirstOrDefaultAsync())!;
    }

    public async Task<ICollection<Day>> GetDaysAsync(int weekId)
    {
        return await TimetableDB.Days
            .Where(d => d.WeekId == weekId)
            .ToArrayAsync();
    }

    public async Task<Day> GetCurrentDayAsync(int groupId)
    {
        int weekId = (await GetCurrentWeekAsync(groupId)).Id;
        return (await TimetableDB.Days
            .Where(d => d.WeekId == weekId)
            .Where(d => d.Date.DayOfWeek == DateTime.Now.DayOfWeek)
            .FirstOrDefaultAsync())!;
    }

    public async Task<ICollection<Lesson>> GetCurrentDayLessons(int groupId)
    {
        int dayId = (await GetCurrentDayAsync(groupId)).Id;
        return await TimetableDB.Lessons
            .Where(l => l.DayId == dayId)
            .ToArrayAsync();
    }

    public async Task<ICollection<Lesson>> GetDayLessons(int dayId)
    {
        return await TimetableDB.Lessons
            .Where(l => l.DayId == dayId)
            // .Join(TimetableDB.Teachers, l => l.Teacher, t => t.Id)
            .ToArrayAsync();
    }

    private async void ChangeDayLessons(Day day)
    {
        day.Lessons = await GetDayLessons(day.Id);
    }

    public async Task<Week> GetCurrentWeekFullInfo(int groupId)
    {
        Week week = await GetCurrentWeekAsync(groupId);
        week.Days = await GetDaysAsync(week.Id);
        
        foreach(var day in week.Days)
        {
            day.Lessons = await GetDayLessons(day.Id);
        }

        return week;
    }

    public async Task<Day> GetDaysByIdAsync(int groupId)
    {
        int weekId = (await GetCurrentWeekAsync(groupId)).Id;
        return (await TimetableDB.Days.Where(d => d.WeekId == weekId).FirstOrDefaultAsync())!;
    }
}
