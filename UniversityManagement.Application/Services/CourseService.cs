using AutoMapper;
using Microsoft.Extensions.Logging;
using UniversityManagement.Application.EntitiesDto;
using UniversityManagement.Application.Services.Interfaces;
using UniversityManagement.Domain.Entities;
using UniversityManagement.Infrastructure;

namespace UniversityManagement.Application.Services;

public class CourseService : BaseService<Course>, ICourseService
{
    private readonly IMapper _mapper;
    private readonly ILogger<CourseService> _logger;

    public CourseService(UniversityDbContext dbContext, IMapper mapper, ILogger<CourseService> logger)
        : base(dbContext)
    {
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<CourseDto> GetCourseById(int id)
    {
        var course = await GetById(id);

        if (course == null)
        {
            _logger.LogError($"Error in CourseService - GetCourseById while fetching course by Id [{id}]");
            throw new Exception($"There is no course with Id [{id}] in DB set.");
        }

        return _mapper.Map<CourseDto>(course);
    }
    
    public async Task<IEnumerable<CourseDto>> GetCoursesAll()
    {
        var courses = await GetAll();

        if (courses == null || !courses.Any())
        {
            _logger.LogError("Error in CourseService - GetCoursesAll while fetching all courses.");
            throw new Exception("Failed to retrieve course list from DB set.");
        }
        
        return _mapper.Map<IEnumerable<CourseDto>>(courses);
    }
}