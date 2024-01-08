using UniversityManagement.Application.EntitiesDto;
using UniversityManagement.Domain.Entities;

namespace UniversityManagement.Application.Services.Interfaces;

public interface ICourseService : IBaseService<Course>
{
    Task<CourseDto> GetCourseById(int id);
    Task<IEnumerable<CourseDto>> GetCoursesAll();
}