using UniversityManagement.Application.EntitiesDto;
using UniversityManagement.Domain.Entities;

namespace UniversityManagement.Application.Services.Interfaces;

public interface IStudentService : IBaseService<Student>
{
    Task<StudentDto> GetStudentById(int studentId);
    Task<IEnumerable<StudentDto>> GetStudents();
    Task<StudentDto> UpdateStudent(StudentDto studentDto);
    Task<StudentDto> CreateStudent(StudentDto newStudentDto);
    Task DeleteStudent(StudentDto studentDto);
    Task AddStudentToGroup(StudentDto studentDto);
    Task RemoveStudentFromGroup(StudentDto studentDto);
}