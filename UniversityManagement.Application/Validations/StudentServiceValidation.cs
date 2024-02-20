using Microsoft.Extensions.Logging;
using UniversityManagement.Application.Interfaces;
using UniversityManagement.Domain.Entities;
using UniversityManagement.DTO.EntitiesDto;

namespace UniversityManagement.Application.Validations;

public class StudentServiceValidation : IStudentServiceValidation
{
        private readonly ILogger<StudentServiceValidation> _logger;

    public StudentServiceValidation(ILogger<StudentServiceValidation> logger)
    {
        _logger = logger;
    }
    
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
}