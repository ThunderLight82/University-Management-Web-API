using Microsoft.AspNetCore.Mvc;
using UniversityManagement.Application.Interfaces;
using UniversityManagement.DTO.EntitiesDto;

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
    
    //GET [UPDATE GROUP]
    [HttpGet]
    public async Task<IActionResult> UpdateGroup()
    {
        ViewBag.CourseList = await _courseService.GetCourses();

        return View();
    }
    
    //POST [UPDATE GROUP]
    [HttpPost]
    public async Task<IActionResult> UpdateGroup(GroupDto groupDto)
    {
        try
        {
            await _groupService.UpdateGroup(groupDto);

            ViewBag.CourseList = await _courseService.GetCourses();
            
            ViewBag.SuccessMessage = $"Selected group name changed successfully to '{groupDto.Name}'.";
            
            return View();
        }
        catch (Exception ex)
        {
            ViewBag.CourseList = await _courseService.GetCourses();
            
            ViewBag.ErrorMessage = $"An error occurred: {ex.Message}";
                
            return View();
        }
    }
    
    //GET [CREATE GROUP]
    [HttpGet]
    public async Task<IActionResult> CreateGroup()
    {
        ViewBag.CourseList = await _courseService.GetCourses();
        
        return View();
    }
    
    //POST [CREATE GROUP]
    [HttpPost]
    public async Task<IActionResult> CreateGroup(GroupDto newGroupDto)
    {
        try
        {
            await _groupService.CreateGroup(newGroupDto);

            ViewBag.CourseList = await _courseService.GetCourses();
            
            ViewBag.SuccessMessage = $"New group with name '{newGroupDto.Name}' is successfully created within course with Id:[{newGroupDto.CourseId}]";
            
            return View();
        }
        catch (Exception ex)
        {
            ViewBag.CourseList = await _courseService.GetCourses();
            
            ViewBag.ErrorMessage = $"An error occurred: {ex.Message}";
            return View();
        }
    }
    
    //GET [DELETE GROUP]
    [HttpGet]
    public async Task<IActionResult> DeleteGroup()
    {
        ViewBag.CourseList = await _courseService.GetCourses();
        
        return View();
    }
    
    //POST [DELETE GROUP]
    [HttpPost]
    public async Task<IActionResult> DeleteGroup(GroupDto groupDto)
    {
        try
        {
            await _groupService.DeleteGroup(groupDto);

            ViewBag.CourseList = await _courseService.GetCourses();

            ViewBag.SuccessMessage = "Selected group was successfully deleted from course.";
            
            return View();
        }
        catch (Exception ex)
        {
            ViewBag.CourseList = await _courseService.GetCourses();
            
            ViewBag.ErrorMessage = $"An error occurred: {ex.Message}";
                
            return View();
        }
    }
    
    //ADDITIONAL GET
    [HttpGet]
    public async Task<IActionResult> GetGroupsByCourseId(int courseId)
    {
        var groups = await _courseService.GetGroupsByCourseId(courseId);
        
        return Json(groups);
    }
}