using Microsoft.AspNetCore.Mvc;
using UniversityManagement.Application.Interfaces;

namespace UniversityManagement.WebApi.Controllers;

public class HomeController : Controller
{
    private readonly ICourseService _courseService;
    private readonly IGroupService _groupService;

    public HomeController(ICourseService courseService, IGroupService groupService)
    {
        _courseService = courseService;
        _groupService = groupService;
    }

    // GET: /Home
    [HttpGet]
    public async Task<IActionResult> GetCourses(string errorMessage)
    {
        if (!string.IsNullOrEmpty(errorMessage))
        {
            ViewBag.ErrorMessage = errorMessage;
        }
        
        var courses = await _courseService.GetCourses();
        
        return View(courses);
    }

    // NAV GET: /Home/Groups/{courseId}
    [HttpGet]
    public async Task<IActionResult> GetGroups(int courseId)
    {
        try
        {
            var groups = await _courseService.GetGroupsByCourseId(courseId);
            
            return View(groups);
        }
        catch (Exception ex)
        {
             ViewBag.ErrorMessage = $"An error occurred: {ex.Message}";
            
             return RedirectToAction("GetCourses", "Home", new { errorMessage = ViewBag.ErrorMessage });
        }
    }

    // NAV GET: /Home/Students/{groupId}
    [HttpGet]
    public async Task<IActionResult> GetStudents(int groupId)
    {
        try
        {
            var students = await _groupService.GetStudentsByGroupId(groupId);
        
            return View(students);
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = $"An error occurred: {ex.Message}";
            
            return RedirectToAction("GetCourses", "Home", new { errorMessage = ViewBag.ErrorMessage });
        }
    }
}
