using Microsoft.Extensions.Logging;
using UniversityManagement.Application.Interfaces;
using UniversityManagement.Domain.Entities;

namespace UniversityManagement.Application.Validations;

public class CourseServiceValidation : ICourseServiceValidation
{
    private readonly ILogger<CourseServiceValidation> _logger;

    public CourseServiceValidation(ILogger<CourseServiceValidation> logger)
    {
        _logger = logger;
    }
    
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
}