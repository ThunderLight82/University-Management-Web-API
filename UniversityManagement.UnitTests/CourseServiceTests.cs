using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using UniversityManagement.Application.Services;
using UniversityManagement.Application.Services.Interfaces;
using UniversityManagement.DataAccess;
using UniversityManagement.WebApi.AutoMapper;
using Xunit;

namespace UniversityManagement.UnitTests;

public class CourseServiceTests
{
    private readonly Mock<ILogger<CourseService>> _mockLoggerService;
    private readonly Mock<ILogger<ValidationService>> _mockLoggerValidationService;
    private readonly IMapper _testMapper;
    private readonly UniversityDbContext _dbContext;
    private readonly UniversityDbContext _emptyDbContext;
    private readonly IValidationService _validationService;
    private readonly ICourseService _courseService;
    
    public CourseServiceTests()
    {
        _mockLoggerService = new Mock<ILogger<CourseService>>();
        _mockLoggerValidationService = new Mock<ILogger<ValidationService>>();
        
        _testMapper = new MapperConfiguration(cfg => cfg
            .AddProfile(new EntitiesMapper()))
            .CreateMapper();

        _dbContext = CreateAndSeedTestDb();

        _validationService = new ValidationService(_mockLoggerValidationService.Object);
        
        _courseService = new CourseService(_dbContext, _testMapper, _mockLoggerService.Object, _validationService);
        
        //Separate empty DB for null or empty cases.  
        _emptyDbContext = CreateEmptyTestDb(); 
    }

    #region LogicTests

    [Fact]
    public async Task GetCoursesAsList_CoursesExist_ReturnCorrectCoursesAsListFromRepo()
    {
        //Arrange & Act
        var courseListResult = await _courseService.GetCourses();

        //Assert
        Assert.NotNull(courseListResult);
        Assert.Equal(5, courseListResult.Count());
        Assert.Contains(courseListResult, c => c.Name == "System Engineer");
        Assert.Contains(courseListResult, c => c.Name == "Data Science");
        Assert.Contains(courseListResult, c => c.Name == "Cyber Security");
    }

    [Fact]
    public async Task GetCourseById_CourseExist_ReturnCorrectCourseByIdFromRepo()
    {
        // Arrange
        var courseId = 5;

        // Act
        var courseIdResult = await _courseService.GetCourseById(courseId);

        // Assert
        Assert.NotNull(courseIdResult);
        Assert.Equal(courseId, courseIdResult.Id);
        Assert.Equal("Cyber Security", courseIdResult.Name);
    }
    
    [Fact]
    public async Task GetCourseById_NotDataFountInDbOrNull_ThrowException()
    {
        // Arrange
        var courseService = new CourseService(_emptyDbContext, _testMapper, _mockLoggerService.Object, _validationService);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(async () => await courseService.GetCourseById(default));
        await Assert.ThrowsAsync<Exception>(async () => await courseService.GetCourseById(0));
    }
    
    [Fact]
    public async Task GetGroupsByCourseId_GroupsAndCourseExist_ReturnCorrectGroupsByCourseIdFromRepo()
    {
        // Arrange & Act
        var groups = await _courseService.GetGroupsByCourseId(2);

        // Assert
        Assert.NotNull(groups);
        Assert.Single(groups);
        Assert.Contains(groups, g => g.Name == "SWE-32");
    }
    
    [Fact]
    public async Task GetGroupsByCourseId_GroupsAndCourseExist_ReturnAnotherCorrectGroupsByCourseIdFromRepo()
    { 
        // Arrange & Act
        var groups = await _courseService.GetGroupsByCourseId(4);

        // Assert
        Assert.NotNull(groups);
        Assert.Equal(2, groups.Count());
        Assert.Contains(groups, g => g.Name == "DA-12");  
        Assert.Contains(groups, g => g.Name == "DA-41");
    }
    
    [Fact]
    public async Task GetGroupsByCourseId_CourseExistButItsEmpty_ThrowException()
    { 
        // Arrange & Act & Assert
        var exception =
            await Assert.ThrowsAsync<Exception>(async () => await _courseService.GetGroupsByCourseId(5));
        
        Assert.Equal("There are no groups for course with Id [5] in DB set.", exception.Message);
    }
    
    [Fact]
    public async Task GetGroupsByCourseId_NotDataFoundInDbOrNull_ThrowException()
    { 
        // Arrange
        var courseService = new CourseService(_emptyDbContext, _testMapper, _mockLoggerService.Object, _validationService);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(async () => await courseService.GetGroupsByCourseId(0));
        await Assert.ThrowsAsync<NullReferenceException>(async () => await courseService.GetGroupsByCourseId(1));
        await Assert.ThrowsAsync<NullReferenceException>(async () => await courseService.GetGroupsByCourseId(123));
        await Assert.ThrowsAsync<NullReferenceException>(async () => await courseService.GetGroupsByCourseId(-1));
    }

    #endregion

    #region DbSetupAndDataSeeding

    private UniversityDbContext CreateAndSeedTestDb()
    {
        var options = new DbContextOptionsBuilder<UniversityDbContext>()
            .UseInMemoryDatabase(databaseName: "InMemory_UniversityServerDB_21")
            .Options;
        
        // Add seed data to the in-memory database for tests cases. 
        var dbContext = new UniversityDbContext(options);
        
        dbContext.Courses.AddRange(
            new() { Id = 1, Name = "System Engineer" },
            new() { Id = 2, Name = "Software Engineer" },
            new() { Id = 3, Name = "Data Science" },
            new() { Id = 4, Name = "Data Analysis" },
            new() { Id = 5, Name = "Cyber Security" }
        );
        
        dbContext.Groups.AddRange(
            new() { Id = 3, Name = "DA-12", CourseId = 4 },
            new() { Id = 4, Name = "DA-41", CourseId = 4 },
            new() { Id = 13, Name = "SWE-32", CourseId = 2 }
        );

        dbContext.SaveChangesAsync();

        return new UniversityDbContext(options);
    }
    
    private UniversityDbContext CreateEmptyTestDb()
    {
        var options = new DbContextOptionsBuilder<UniversityDbContext>()
            .UseInMemoryDatabase(databaseName: "InMemory_EmptyUniversityServerDB_21")
            .Options;

        return new UniversityDbContext(options);
    }
    
    #endregion
}

