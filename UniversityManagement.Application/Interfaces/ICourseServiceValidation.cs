using UniversityManagement.Domain.Entities;

namespace UniversityManagement.Application.Interfaces;

public interface ICourseServiceValidation
{
    void ValidateGetCourseById(int courseId);
    void ValidateGetCourses(IEnumerable<Course> courses);
    void ValidateGetGroupsByCourseId(int courseId);
}