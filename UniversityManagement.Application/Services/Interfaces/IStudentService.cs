using UniversityManagement.Application.EntitiesDto;
using UniversityManagement.Domain.Entities;

namespace UniversityManagement.Application.Services.Interfaces;

public interface IStudentService : IBaseService<Student>
{
    Task<StudentDto> GetStudentById(int studentId);
    Task<IEnumerable<StudentDto>> GetStudentsAll();
    Task<IEnumerable<StudentDto>> GetStudentsAllWithGroupName();
    Task<IEnumerable<StudentDto>> GetStudentsAllByGroupId(int groupId);
    Task<StudentDto> ChangeStudentFirstName(string newChangedFirstName, int studentId);
    Task<StudentDto> ChangeStudentLastName(string newChangedLastName, int studentId);
    Task<StudentDto> CreateStudent(StudentDto newStudentDto, string newStudentFirstName, string newStudentLastName);
    Task DeleteStudent(int studentId);
    Task AddStudentToGroup(int studentId, int groupId);
    Task RemoveStudentFromGroup(int studentId);
}