using Microsoft.AspNetCore.Mvc;
using UniversityManagement.Application.Services.Interfaces;

namespace UniversityManagement.WebApi.Controllers;

public class HomeController : Controller
{
    private readonly ICourseService _courseService;
    private readonly IGroupService _groupService;
    private readonly IStudentService _studentService;

    public HomeController(ICourseService courseService, IGroupService groupService, IStudentService studentService)
    {
        _courseService = courseService;
        _groupService = groupService;
        _studentService = studentService;
    }

    // GET: /Home
    [HttpGet]
    public async Task<IActionResult> GetCoursesAll(string errorMessage)
    {
        if (!string.IsNullOrEmpty(errorMessage))
        {
            ViewBag.ErrorMessage = errorMessage;
        }
        
        var courses = await _courseService.GetCoursesAll();
        
        return View(courses);
    }

    // NAV GET: /Home/Groups/{courseId}
    [HttpGet]
    public async Task<IActionResult> GetGroupsAll(int courseId)
    {
        try
        {
            var groups = await _groupService.GetGroupsAllByCourseId(courseId);
            
            return View(groups);
        }
        catch (Exception ex)
        {
             ViewBag.ErrorMessage = $"An error occurred: {ex.Message}";
            
             return RedirectToAction("GetCoursesAll", "Home", new { errorMessage = ViewBag.ErrorMessage });
        }
    }

    // NAV GET: /Home/Students/{groupId}
    [HttpGet]
    public async Task<IActionResult> GetStudentsAll(int groupId)
    {
        try
        {
            var students = await _studentService.GetStudentsAllByGroupId(groupId);
        
            return View(students);
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = $"An error occurred: {ex.Message}";
            
            return RedirectToAction("GetCoursesAll", "Home", new { errorMessage = ViewBag.ErrorMessage });
        }
    }
}
