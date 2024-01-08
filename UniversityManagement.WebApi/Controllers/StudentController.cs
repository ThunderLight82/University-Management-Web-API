using Microsoft.AspNetCore.Mvc;
using UniversityManagement.Application.EntitiesDto;
using UniversityManagement.Application.Services.Interfaces;

namespace UniversityManagement.WebApi.Controllers;

public class StudentController : Controller
{
    private readonly IStudentService _studentService;
    private readonly IGroupService _groupService;
    
    public StudentController(IStudentService studentService, IGroupService groupService)
    {
        _studentService = studentService;
        _groupService = groupService;
    }
    
    //GET [EDIT STUDENT]
    [HttpGet]
    public async Task<IActionResult>EditStudent()
    {
        ViewBag.StudentsList = await _studentService.GetStudentsAll();
        
        return View();
    }
    
    //ADDITIONAL GET [EDIT STUDENT]
    [HttpGet]
    public async Task<IActionResult>GetStudentNameDetails(int studentId)
    {
        var studentDetails = await _studentService.GetStudentById(studentId);
        
        return Json(new { firstName = studentDetails.FirstName, lastName = studentDetails.LastName });
    }
    
    //POST [EDIT STUDENT]
    [HttpPost]
    public async Task<IActionResult> EditStudent(string newChangedFirstName, string newChangedLastName, int studentId)
    {
        try
        {
            await _studentService.ChangeStudentFirstName(newChangedFirstName, studentId);
            await _studentService.ChangeStudentLastName(newChangedLastName, studentId);
            
            ViewBag.StudentsList = await _studentService.GetStudentsAll();
                    
            ViewBag.SuccessMessage = $"Selected student name changed successfully to '{newChangedFirstName} " + $"{newChangedLastName}'.";
            
            return View();
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = $"An error occurred: {ex.Message}";
            
            return View();
        }
    }
    
    //GET [CREATE STUDENT]
    [HttpGet]
    public IActionResult CreateStudent()
    {
        return View();
    }
    
    //POST [CREATE STUDENT]
    [HttpPost]
    public async Task<IActionResult> CreateStudent(StudentDto newStudentDto, string newStudentFirstName, string newStudentLastName)
    {
        try
        {
            await _studentService.CreateStudent(newStudentDto, newStudentFirstName, newStudentLastName);
            
            ViewBag.SuccessMessage = $"New student with name '{newStudentFirstName} " + $"{newStudentLastName}' is successfully created.";
            
            return View();
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = $"An error occurred: {ex.Message}";
            
            return View();
        }
    }
    
    //GET [DELETE STUDENT]
    [HttpGet]
    public async Task<IActionResult> DeleteStudent()
    { 
        ViewBag.StudentsList = await _studentService.GetStudentsAll();
        
        return View();
    }
    
    //POST [DELETE STUDENT]
    [HttpPost]
    public async Task<IActionResult> DeleteStudent(int studentId)
    {
        try
        {
            await _studentService.DeleteStudent(studentId);
            
            ViewBag.StudentsList = await _studentService.GetStudentsAll();
            
            ViewBag.SuccessMessage = $"student with Id [{studentId}] is successfully deleted.";
            
            return View();
        }
        catch (Exception ex)
        {
            ViewBag.StudentsList = await _studentService.GetStudentsAll();
            
            ViewBag.ErrorMessage = $"An error occurred: {ex.Message}";
            
            return View();
        }
    }
    
    //GET [MANAGE STUDENTS GROUP]
    [HttpGet]
    public async Task<IActionResult> ManageStudentsGroup()
    {
        var studentsWithGroupName = await _studentService.GetStudentsAllWithGroupName();
        
        ViewBag.GroupsList = await _groupService.GetGroupsAll();
        
        return View(studentsWithGroupName);
    }
    
    //POST [MANAGE STUDENTS GROUP]
    [HttpPost]
    public async Task<IActionResult> ManageStudentsGroup(int studentId, int groupId, string buttonAction)
    {
        try
        {
            switch (buttonAction)
            {
                case"addStudentToGroup":
                    await _studentService.AddStudentToGroup(studentId, groupId);
                    break;
                
                case "removeStudentFromGroup":
                    await _studentService.RemoveStudentFromGroup(studentId);
                    break;
            }
            
            var students = await _studentService.GetStudentsAll();
            
            ViewBag.GroupsList = await _groupService.GetGroupsAll();
            
            return View(students);
        }
        catch (Exception)
        {
            var students = await _studentService.GetStudentsAll();

            ViewBag.GroupsList = await _groupService.GetGroupsAll();

            return View(students);
        }
    }
}

