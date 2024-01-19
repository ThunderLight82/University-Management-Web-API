using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UniversityManagement.Application.EntitiesDto;
using UniversityManagement.Application.Services.Interfaces;
using UniversityManagement.Domain.Entities;
using UniversityManagement.DataAccess;

namespace UniversityManagement.Application.Services;

public class CourseService : BaseService<Course>, ICourseService
{
    private readonly IMapper _mapper;
    private readonly ILogger<CourseService> _logger;
    private readonly IValidationService _validationService;

    public CourseService(UniversityDbContext dbContext, IMapper mapper, ILogger<CourseService> logger, IValidationService validationService)
        : base(dbContext)
    {
        _mapper = mapper;
        _logger = logger;
        _validationService = validationService;
    }

    public async Task<CourseDto> GetCourseById(int courseId)
    {
        _validationService.ValidateGetCourseById(courseId);
            
        var course = await GetById(courseId);
        
        return _mapper.Map<CourseDto>(course);
    }
    
    public async Task<IEnumerable<CourseDto>> GetCourses()
    {
        var courses = await GetAll();
        
        _validationService.ValidateGetCourses(courses);
        
        return _mapper.Map<IEnumerable<CourseDto>>(courses);
    }
    
    public async Task<IEnumerable<GroupDto>> GetGroupsByCourseId(int courseId)
    {
        _validationService.ValidateGetGroupsByCourseId(courseId);
        
        var course = await _dbContext.Courses
            .Where(c => c.Id == courseId)
            .Include(c => c.Groups)
            .FirstOrDefaultAsync();

        if (course!.Groups.Count == 0)
        {
            _logger.LogError($"Error in CourseService - GetGroupsListByCourseId while fetching groups by course Id [{courseId}]");
            throw new Exception($"There are no groups for course with Id [{courseId}] in DB set.");
        }
        
        return _mapper.Map<List<GroupDto>>(course.Groups);
    }
}