using UniversityManagement.Application.EntitiesDto;
using UniversityManagement.Domain.Entities;

namespace UniversityManagement.Application.Services.Interfaces;

public interface IValidationService
{
    #region StudentValidation
    
    void ValidateGetStudentById(int studentId);
    void ValidateGetStudents(IEnumerable<Student> students);
    void ValidateUpdateStudent(StudentDto studentDto);
    void ValidateDeleteStudent(StudentDto studentDto);
    void ValidateCreateStudent(StudentDto newStudentDto);
    public void ValidateStudentGroupOperation(StudentDto studentDto);
    
    #endregion

    #region GroupValidation

    void ValidateGetGroupsById(int groupId);
    void ValidateGetGroups(IEnumerable<Group> groups);
    public void ValidateGetStudentsByGroupId(int groupId);
    void ValidateUpdateGroup(GroupDto groupDto);
    void ValidateCreateGroup(GroupDto newGroupDto);
    void ValidateDeleteGroup(GroupDto groupDto);

    #endregion

    #region CourseValidation

    void ValidateGetCourseById(int courseId);
    void ValidateGetCourses(IEnumerable<Course> courses);
    void ValidateGetGroupsByCourseId(int courseId);

    #endregion
}