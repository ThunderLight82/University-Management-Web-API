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
    
    //GET [UPDATE STUDENT]
    [HttpGet]
    public async Task<IActionResult>UpdateStudent()
    {
        ViewBag.StudentsList = await _studentService.GetStudents();
        
        return View();
    }
    
    //POST [UPDATE STUDENT]
    [HttpPost]
    public async Task<IActionResult> UpdateStudent(StudentDto studentDto)
    {
        try
        {
            await _studentService.UpdateStudent(studentDto);
            
            ViewBag.StudentsList = await _studentService.GetStudents();
                    
            ViewBag.SuccessMessage = $"Selected student name changed successfully to '{studentDto.FirstName} " + $"{studentDto.LastName}'.";
            
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
    public async Task<IActionResult> CreateStudent(StudentDto newStudentDto)
    {
        try
        {
            await _studentService.CreateStudent(newStudentDto);
            
            ViewBag.SuccessMessage = $"New student with name '{newStudentDto.FirstName} " + $"{newStudentDto.LastName}' is successfully created.";
            
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
        ViewBag.StudentsList = await _studentService.GetStudents();
        
        return View();
    }
    
    //POST [DELETE STUDENT]
    [HttpPost]
    public async Task<IActionResult> DeleteStudent(StudentDto studentDto)
    {
        try
        {
            await _studentService.DeleteStudent(studentDto);
            
            ViewBag.StudentsList = await _studentService.GetStudents();
            
            ViewBag.SuccessMessage = $"student with Id [{studentDto.Id}] is successfully deleted.";
            
            return View();
        }
        catch (Exception ex)
        {
            ViewBag.StudentsList = await _studentService.GetStudents();
            
            ViewBag.ErrorMessage = $"An error occurred: {ex.Message}";
            
            return View();
        }
    }
    
    //GET [MANAGE STUDENTS GROUP]
    [HttpGet]
    public async Task<IActionResult> ManageStudentsGroup()
    {
        var students = await _studentService.GetStudents();
        
        ViewBag.GroupsList = await _groupService.GetGroups();
        
        return View(students);
    }
    
    //POST [MANAGE STUDENTS GROUP - ADD STUDENT TO GROUP]
    [HttpPost]
    public async Task<IActionResult> AddStudentToGroup(StudentDto studentDto)
    {
        try
        {
            await _studentService.AddStudentToGroup(studentDto);

            var students = await _studentService.GetStudents();
            
            ViewBag.GroupsList = await _groupService.GetGroups();
            
            return View("ManageStudentsGroup", students);
        }
        catch (Exception)
        {
            var students = await _studentService.GetStudents();

            ViewBag.GroupsList = await _groupService.GetGroups();

            return View("ManageStudentsGroup", students);
        }
    }
    
    //POST [MANAGE STUDENTS GROUP - REMOVE STUDENT FROM GROUP]
    [HttpPost]
    public async Task<IActionResult> RemoveStudentFromGroup(StudentDto studentDto)
    {
        try
        {
            await _studentService.RemoveStudentFromGroup(studentDto);
            
            var students = await _studentService.GetStudents();
            
            ViewBag.GroupsList = await _groupService.GetGroups();
            
            return View("ManageStudentsGroup", students);
        }
        catch (Exception)
        {
            var students = await _studentService.GetStudents();

            ViewBag.GroupsList = await _groupService.GetGroups();

            return View("ManageStudentsGroup", students);
        }
    }
    
    //ADDITIONAL GET
    [HttpGet]
    public async Task<IActionResult> GetStudentNameDetails(int studentId)
    {
        var studentDetails = await _studentService.GetStudentById(studentId);
        
        return Json(new { firstName = studentDetails.FirstName, lastName = studentDetails.LastName });
    }
}

