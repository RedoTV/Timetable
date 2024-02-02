using TimetableServer.Database;
using TimetableServer.Models.DbModels;
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

    public async Task<bool> AddFaculties(ICollection<Faculty> faculties)
    {
        _logger.LogInformation("Adding Faculties ...");
        try
        {
            await TimetableDB.Faculties.AddRangeAsync(faculties);
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

    public async Task<bool> AddSemester(Semester semester)
    {
        _logger.LogInformation("Adding Semester...");
        try
        {
            TimetableDB.Faculties
                .Where(f => f.Id == semester.FacultyId)
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

    public async Task<bool> AddGroups(ICollection<Group> groups)
    {
        try
        {
            if(groups.Count() == 0)
                return false;

            _logger.LogInformation("Take semesterId from all groups");
            int semesterId = groups.Where(gr => gr.SemesterId != 0).First()!.SemesterId;

            _logger.LogInformation("Find semester by semesterId");
            Semester semester = TimetableDB.Semesters.Where(s => s.Id == semesterId).First();

            _logger.LogInformation("Adding Groups...");
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
            Semester semester = TimetableDB.Semesters.Find(semesterId)!;

            _logger.LogInformation("getting all groups by semesterID");
            ICollection<Group> groups = TimetableDB.Groups
                .Where(gr => gr.SemesterId == semesterId)!.ToList();
            
            _logger.LogInformation("Initializing all weeks...");
            await AddWeeksForAllGroups(firstWeekIsOdd,semester,groups);

            _logger.LogInformation("Weeks are initialized");

            
        }
        catch(Exception exc){
            _logger.LogCritical("weeks aren't initialized: " + exc.Message);
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

    public Task<bool> AddLessons(ICollection<Lesson> lessons, bool firstWeekIsOdd, int group)
    {
        // ICollection<Week> weeks;
        // if(firstWeekIsOdd == true)
        //     weeks = TimetableDB.Weeks.Where(week => week.IsOdd == firstWeekIsOdd).ToList();
        // else
        //     weeks = TimetableDB.Weeks.Where(week => week.IsOdd == firstWeekIsOdd).ToList();
        throw new NotImplementedException();
    }

    public Task<bool> AddTeachers(ICollection<Teacher> teacher)
    {
        throw new NotImplementedException();
    }

    public Task GetFacultiesName()
    {
        throw new NotImplementedException();
    }

    public Task GetSemesters()
    {
        throw new NotImplementedException();
    }

    public Task GetWeekById(int weekId)
    {
        throw new NotImplementedException();
    }

    public Task GetCurrentWeek()
    {
        throw new NotImplementedException();
    }
}
