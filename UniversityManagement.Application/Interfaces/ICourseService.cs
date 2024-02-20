using UniversityManagement.DTO.EntitiesDto;
using UniversityManagement.Domain.Entities;

namespace UniversityManagement.Application.Services.Interfaces;

public interface ICourseService : IBaseService<Course>
{
    Task<CourseDto> GetCourseById(int courseId);
    Task<IEnumerable<CourseDto>> GetCourses();
    Task<IEnumerable<GroupDto>> GetGroupsByCourseId(int courseId);
}