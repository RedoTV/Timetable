using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TimetableServer.HelperClasses;
using TimetableServer.Models.DbModels;
using TimetableServer.Models.Requests;
using TimetableServer.Models.Responses;
using TimetableServer.Services.Implementations;
using TimetableServer.Services.Interfaces;

namespace TimetableServer;

[ApiController]
[Route("[controller]")]
public class TimetableController : ControllerBase
{
    private ILessonService LessonService;
    public TimetableController(ILessonService lessonService)
    {
        LessonService = lessonService;
    }

    [HttpGet]
    [Route("addFaculties")]
    public async Task<IActionResult> AddFaculties()
    {
        ICollection<Faculty> faculties = new List<Faculty>(){
                new Faculty(){ Name = "Math"},
                new Faculty(){ Name = "Physics"}
            };
        return Ok(await LessonService.AddFaculties(faculties));
    }

    [HttpGet]
    [Route("addSemester")]
    public async Task<IActionResult> AddSemester()
    {
        Semester semester = new Semester(){
            FromDate = new DateTime(24, 1,24),
            ToDate = new DateTime(27,06,27),
            FacultyId = 3
        };
        return Ok(await LessonService.AddSemester(semester));
    }

    [HttpGet]
    [Route("addGroups")]
    public async Task<IActionResult> AddGroups()
    {
        ICollection<Group> groups = new List<Group>(){
            new Group(){
                Name = "PRO-12",
                SemesterId = 1
            },
            new Group(){
                Name = "PRO-11",
                SemesterId = 1
            },
            new Group(){
                Name = "ITP-11",
                SemesterId = 1
            }
        };  
        return Ok(await LessonService.AddGroups(groups));
    }

    [HttpGet]
    [Route("initializeWeeks")]
    public async Task<IActionResult> InitializeWeeks()
    {
        return Ok(await LessonService.InitializeSemesterWeeks(1, true));
    }

    [HttpGet]
    [Route("initializeDays")]
    public async Task<IActionResult> InitializeDays()
    {
        return Ok(await LessonService.InitializeWeekDays(1));
    }

    // [HttpGet]
    // [Route("getCurrentWeek")]
    // public async Task<WeekModelResponse> GetCurrentWeek()
    // {
        
    // }

    // [HttpGet]
    // [Route("getAllWeeks")]
    // public async Task<WeekModelResponse[]> GetAllWeeks()
    // {
        
    // }

    // [HttpGet]
    // [Route("getWeek/{weekNumber}")]
    // public async Task<WeekModelResponse> GetWeek(int weekNumber)
    // {
        
    // }

    // [Authorize(Roles = RolesEnum.User)]
    // [HttpPost]
    // [Route("addDay")]
    // public IActionResult AddDayInWeek(DayModelRequest day)
    // {
        
    // }

    // [HttpPost]
    // [Route("initialize")]
    // public async Task<IActionResult> InitializeTable(DateTime dateOfStart, DateTime dateOfEnd)
    // {
        
    // }
}
