using Microsoft.AspNetCore.Mvc;
using UniversityManagement.Application.EntitiesDto;
using UniversityManagement.Application.Services.Interfaces;

namespace UniversityManagement.WebApi.Controllers;

public class GroupController : Controller
{
    private readonly IGroupService _groupService;
    private readonly ICourseService _courseService;

    public GroupController(IGroupService groupService, ICourseService courseService)
    {
        _groupService = groupService;
        _courseService = courseService;
    }
    
    //GET [EDIT GROUP]
    [HttpGet]
    public async Task<IActionResult> EditGroup()
    {
        ViewBag.CourseList = await _courseService.GetCoursesAll();

        return View();
    }
    
    //ADDITIONAL GET [EDIT GROUP]
    [HttpGet]
    public async Task<IActionResult> GetGroupsByCourseId(int courseId)
    {
        var groups = await _groupService.GetGroupsAllByCourseId(courseId);
        
        return Json(groups);
    }
    
    //POST [EDIT GROUP]
    [HttpPost]
    public async Task<IActionResult> EditGroup(string newChangedGroupName, int groupId)
    {
        try
        {
            await _groupService.ChangeGroupName(newChangedGroupName, groupId);

            ViewBag.CourseList = await _courseService.GetCoursesAll();
            
            ViewBag.SuccessMessage = $"Selected group name changed successfully to '{newChangedGroupName}'.";
            
            return View();
        }
        catch (Exception ex)
        {
            ViewBag.CourseList = await _courseService.GetCoursesAll();
            
            ViewBag.ErrorMessage = $"An error occurred: {ex.Message}";
                
            return View();
        }
    }
    
    //GET [CREATE GROUP]
    [HttpGet]
    public async Task<IActionResult> CreateGroup()
    {
        ViewBag.CourseList = await _courseService.GetCoursesAll();
        
        return View();
    }
    
    //POST [CREATE GROUP]
    [HttpPost]
    public async Task<IActionResult> CreateGroup(GroupDto newGroupDto, string newGroupName, int courseId)
    {
        try
        {
            await _groupService.CreateGroup(newGroupDto, newGroupName, courseId);

            ViewBag.CourseList = await _courseService.GetCoursesAll();
            
            ViewBag.SuccessMessage = $"New group with name '{newGroupName}' is successfully created within course with Id:[{courseId}]";
            
            return View();
        }
        catch (Exception ex)
        {
            ViewBag.CourseList = await _courseService.GetCoursesAll();
            
            ViewBag.ErrorMessage = $"An error occurred: {ex.Message}";
            return View();
        }
    }
    
    //GET [DELETE GROUP]
    [HttpGet]
    public async Task<IActionResult> DeleteGroup()
    {
        ViewBag.CourseList = await _courseService.GetCoursesAll();
        
        return View();
    }
    
    //POST [DELETE GROUP]
    [HttpPost]
    public async Task<IActionResult> DeleteGroup(int groupId)
    {
        try
        {
            await _groupService.DeleteGroup(groupId);

            ViewBag.CourseList = await _courseService.GetCoursesAll();

            ViewBag.SuccessMessage = "Selected group was successfully deleted from course.";
            
            return View();
        }
        catch (Exception ex)
        {
            ViewBag.CourseList = await _courseService.GetCoursesAll();
            
            ViewBag.ErrorMessage = $"An error occurred: {ex.Message}";
                
            return View();
        }
    }
}