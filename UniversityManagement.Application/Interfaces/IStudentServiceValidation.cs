using UniversityManagement.Domain.Entities;
using UniversityManagement.DTO.EntitiesDto;

namespace UniversityManagement.Application.Interfaces;

public interface IStudentServiceValidation
{
    void ValidateGetStudentById(int studentId);
    void ValidateGetStudents(IEnumerable<Student> students);
    void ValidateUpdateStudent(StudentDto studentDto);
    void ValidateDeleteStudent(StudentDto studentDto);
    void ValidateCreateStudent(StudentDto newStudentDto);
    public void ValidateStudentGroupOperation(StudentDto studentDto);
}