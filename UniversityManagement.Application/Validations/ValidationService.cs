using Microsoft.Extensions.Logging;
using UniversityManagement.Application.Services.Interfaces;
using UniversityManagement.Domain.Entities;
using UniversityManagement.DTO.EntitiesDto;

namespace UniversityManagement.Application.Validations;

public class ValidationService : IValidationService
{
    private readonly ILogger<ValidationService> _logger;

    public ValidationService(ILogger<ValidationService> logger)
    {
        _logger = logger;
    }

    #region StudentValidation

    public void ValidateGetStudentById(int studentId)
    {
        if (studentId == default)
        {
            _logger.LogError($"Error in StudentService - GetStudentById while fetching student by Id [{studentId}]");
            throw new Exception($"There is no student with Id [{studentId}] in DB set.");
        }
    }
    
    public void ValidateGetStudents(IEnumerable<Student> students)
    {
        if (students == null)
        {
            _logger.LogError("Error in StudentService - GetGetStudents while fetching student list from DB.");
            throw new Exception("Failed to retrieve students list from DB set.");
        }
    }
    
    public void ValidateUpdateStudent(StudentDto studentDto)
    {
        if (studentDto.Id == default)
        {
            _logger.LogError($"Error in StudentService - UpdateStudent while fetching student by Id [{studentDto.Id}]");
            throw new Exception($"There is no student with Id [{studentDto.Id}] in DB set.");
        }

        if (string.IsNullOrWhiteSpace(studentDto.FirstName) || string.IsNullOrWhiteSpace(studentDto.LastName))
        {
            _logger.LogError("Error in StudentService - UpdateStudent while updating student first/last name.");
            throw new Exception("First and/or last name are required for fields to complete operation.");
        }
    }
    
    public void ValidateCreateStudent(StudentDto newStudentDto)
    {
        if (string.IsNullOrWhiteSpace(newStudentDto.FirstName) || string.IsNullOrWhiteSpace(newStudentDto.LastName))
        {
            _logger.LogError("Error in StudentService - CreateStudent while creating new student.");
            throw new Exception("First and last name are required for fields to complete operation.");
        }
    }

    public void ValidateDeleteStudent(StudentDto studentDto)
    {
        if (studentDto.Id == default)
        {
            _logger.LogError("Error in StudentService - DeleteStudent while deleting selected student from DB.");
            throw new Exception("Student Id cannot be null for delete operation.");
        }
    }
    
    public void ValidateStudentGroupOperation(StudentDto studentDto)
    {
        if (studentDto.Id == default)
        {
            _logger.LogError("Error in StudentService - AddStudentToGroup while adding student to group.");
            throw new Exception("Student Id cannot be null for this operation.");
        }
    }
    
    #endregion

    #region GroupServiceValidation

    public void ValidateGetGroupsById(int groupId)
    {
        if (groupId == default)
        {
            _logger.LogError($"Error in GroupService - GetGroupById while fetching group by Id [{groupId}]");
            throw new Exception($"There is no group with Id [{groupId}] in DB set.");
        }
    }
    
    public void ValidateGetGroups(IEnumerable<Group> groups)
    {
        if (groups == null)
        {
            _logger.LogError("Error in GroupService - GetGroups while fetching all group.");
            throw new Exception("Failed to retrieve group list from DB set.");
        }
    }
    
    public void ValidateGetStudentsByGroupId(int groupId)
    {
        if (groupId == default)
        {
            _logger.LogError($"Error in GroupService - GetStudentsListByGroupId while fetching students by group Id [{groupId}]");
            throw new Exception($"There is no group with Id [{groupId}] in DB set.");
        }
    }
    
    public void ValidateUpdateGroup(GroupDto groupDto)
    {
        if (groupDto.Id == default)
        {
            _logger.LogError($"Error in GroupService - UpdateGroup while fetching group with Id [{groupDto.Id}]");
            throw new Exception($"There is no group with Id [{groupDto.Id}] in DB set.");
        }

        if (string.IsNullOrWhiteSpace(groupDto.Name))
        {
            _logger.LogError($"Error in GroupService - UpdateGroup new group name is null or empty for group with Id [{groupDto.Id}]");
            throw new Exception($"New group name is null or empty for group with Id [{groupDto.Id}]");
        }
    }
    
    public void ValidateCreateGroup(GroupDto newGroupDto)
    {
        if (newGroupDto.CourseId == default)
        {
            _logger.LogError($"Error in GroupService - CreateGroup while fetching course with Id [{newGroupDto.CourseId}]");
            throw new Exception($"There is no course with Id [{newGroupDto.CourseId}] in DB set.");
        }
        
        if (string.IsNullOrWhiteSpace(newGroupDto.Name))
        {
            _logger.LogError("Error in GroupService - CreateGroup new created group name is null or empty.");
            throw new Exception("New created group name is null or empty.");
        }
    }

    public void ValidateDeleteGroup(GroupDto groupDto)
    {
        if (groupDto.Id == default)
        {
            _logger.LogError($"Error in GroupService - DeleteGroup while fetching group with Id [{groupDto.Id}]");
            throw new Exception($"There is no group with Id [{groupDto.Id}] in DB set.");
        }
    }

    #endregion

    #region CourseServiceValidation
    
    public void ValidateGetCourseById(int courseId)
    {
        if (courseId == default)
        {
            _logger.LogError($"Error in CourseService - GetCourseById while fetching course by Id [{courseId}]");
            throw new Exception($"There is no course with Id [{courseId}] in DB set.");
        }
    }
    
    public void ValidateGetCourses(IEnumerable<Course> courses)
    {
        if (courses == null)
        {
            _logger.LogError("Error in CourseService - GetCourses while fetching all courses.");
            throw new Exception("Failed to retrieve course list from DB set.");
        }
    }
    
    public void ValidateGetGroupsByCourseId(int courseId)
    {
        if (courseId == default)
        {
            _logger.LogError($"Error in CourseService - GetGroupsListByCourseId while fetching groups by course Id [{courseId}]");
            throw new Exception($"There is no course with Id [{courseId}] in DB set.");
        }
    }
    
    #endregion
}