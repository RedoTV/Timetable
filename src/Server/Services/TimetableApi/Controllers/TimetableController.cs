﻿using Microsoft.AspNetCore.Mvc;
using TimetableServer.Models.DbModels;
using TimetableServer.Models.Requests;
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
    [Route("getFacultiesName")]
    public async Task<IEnumerable<string>> GetFacultiesName()
    {
        return await LessonService.GetFacultiesNameAsync();
    }

    [HttpGet]
    [Route("getSemesters")]
    public async Task<IEnumerable<Semester>> GetSemesters(int facultyId)
    {
        return await LessonService.GetSemestersAsync(facultyId);
    }

    [HttpGet]
    [Route("getCurrentWeek")]
    public async Task<Week> GetCurrentWeek(int groupId)
    {
        return await LessonService.GetCurrentWeekAsync(groupId);
    }

    [HttpGet]
    [Route("getCurrentWeekFullInfo")]
    public async Task<Week> GetCurrentWeekFullInfo(int groupId)
    {
        return await LessonService.GetCurrentWeekFullInfo(groupId);
    }

    [HttpPost]
    [Route("addFaculties")]
    public async Task<IActionResult> AddFaculties(string[] facultiesName)
    {
        var response = await LessonService.AddFaculties(facultiesName);
        if(response == false)
            return BadRequest("faculties haven't been added");

        return Ok(response);
    }

    [HttpPost]
    [Route("addSemester")]
    public async Task<IActionResult> AddSemester(SemesterRequestForm semester)
    {
        bool response = await LessonService.AddSemester(semester);
        if(response == false)
            return BadRequest("The semester haven't been added");

        return Ok(response);
    }

    [HttpPost]
    [Route("addGroups")]
    public async Task<IActionResult> AddGroups(GroupRequestForm[] groups)
    {
        bool response = await LessonService.AddGroups(groups);
        if(response == false)
            return BadRequest("Groups haven't been added");

        return Ok(response);
    }

    [HttpPost]
    [Route("initializeWeeks")]
    public async Task<IActionResult> InitializeWeeks(int semesterId, bool firstWeekIsOdd)
    {
        bool response = await LessonService.InitializeSemesterWeeks(semesterId, firstWeekIsOdd);
        if(response == false)
            return BadRequest("Weeks are not initialized");

        return Ok(response);
    }

    [HttpPost]
    [Route("initializeDays")]
    public async Task<IActionResult> InitializeDays(int semesterId)
    {
        bool response = await LessonService.InitializeWeekDays(semesterId);
        if(response == false)
            return BadRequest("Days are not initialized");

        return Ok(response);
    }

    [HttpPost]
    [Route("addTeachers")]
    public async Task<IActionResult> AddTeachers(ICollection<Teacher> teachers)
    {
        bool response = await LessonService.AddTeachers(teachers);
        if(response == false)
            return BadRequest("Teachers haven't been added");

        return Ok(response);
    }

    [HttpPost]
    [Route("addLessons")]
    public async Task<IActionResult> AddLessons(ICollection<LessonRequestForm> lessons, bool firstWeekIsOdd, int groupId)
    {
        bool response = await LessonService.AddLessonsToGroup(lessons, firstWeekIsOdd, groupId);
        if(response == false)
            return BadRequest("Lessons haven't been added");

        return Ok(response);
    }

}
