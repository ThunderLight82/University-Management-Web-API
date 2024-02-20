using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using UniversityManagement.Application.Services;
using UniversityManagement.Application.Services.Interfaces;
using UniversityManagement.Application.Validations;
using UniversityManagement.DataAccess;
using UniversityManagement.DTO.EntitiesDto;
using UniversityManagement.WebApi.AutoMapper;
using Xunit;

namespace UniversityManagement.UnitTests;

public class GroupServiceTests
{
    private readonly Mock<ILogger<GroupService>> _mockLoggerService;
    private readonly Mock<ILogger<ValidationService>> _mockLoggerValidationService;
    private readonly IMapper _testMapper;
    private readonly UniversityDbContext _dbContext;
    private readonly UniversityDbContext _emptyDbContext;
    private readonly IValidationService _validationService;
    private readonly IGroupService _groupService;

    public GroupServiceTests()
    {
        _mockLoggerService = new Mock<ILogger<GroupService>>();
        _mockLoggerValidationService = new Mock<ILogger<ValidationService>>();
        
        _testMapper = new MapperConfiguration(cfg => cfg
                .AddProfile(new EntitiesMapper()))
            .CreateMapper();

        _dbContext = CreateAndSeedTestDb();

        _validationService = new ValidationService(_mockLoggerValidationService.Object);
        
        _groupService = new GroupService(_dbContext, _testMapper, _mockLoggerService.Object, _validationService);

        //Separate empty DB for some null or empty cases.  
        _emptyDbContext = CreateEmptyTestDb();
    }

    #region LogicTest
    
    [Fact]
    public async Task GetStudentsByGroupId_StudentsAndGroupExist_ReturnCorrectStudentsByGroupIdFromRepo()
    {
        // Arrange & Act
        var students = await _groupService.GetStudentsByGroupId(1);

        // Assert
        Assert.NotNull(students);
        Assert.Single(students);
        Assert.Contains(students, s => s.FirstName == "Oleg");
        Assert.Contains(students, s => s.LastName == "Kotlyar");
    }
    
    [Fact]
    public async Task GetStudentsByGroupId_StudentsAndGroupExist_ReturnAnotherCorrectStudentsByGroupIdFromRepo()
    {
        // Arrange & Act
        var students = await _groupService.GetStudentsByGroupId(2);

        // Assert
        Assert.NotNull(students);
        Assert.Equal(2, students.Count());
        Assert.Contains(students, g => g.FirstName == "Adam");  
        Assert.Contains(students, g => g.LastName == "Kishinev");
        Assert.Contains(students, g => g.FirstName == "Sam");  
        Assert.Contains(students, g => g.LastName == "Stone");
    }
    
    [Fact]
    public async Task GetStudentsByGroupId_GroupExistButItsEmpty_ThrowException()
    {
        // Arrange & Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(async () => await _groupService.GetStudentsByGroupId(4));
        Assert.Equal("There are no students for group with Id [4] in DB set.", exception.Message);
    }
    
    [Fact]
    public async Task GetStudentsByGroupId_NotDataFoundInDbOrNull_ThrowException()
    {
        // Arrange
        var groupService = new GroupService(_emptyDbContext, _testMapper, _mockLoggerService.Object, _validationService);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(async () => await groupService.GetStudentsByGroupId(0));
        await Assert.ThrowsAsync<NullReferenceException>(async () => await groupService.GetStudentsByGroupId(1));
        await Assert.ThrowsAsync<NullReferenceException>(async () => await groupService.GetStudentsByGroupId(123));
        await Assert.ThrowsAsync<NullReferenceException>(async () => await groupService.GetStudentsByGroupId(-1));
    }
    
    [Theory]
    [InlineData("New Group Name", 5, true)]       // Valid case
    [InlineData("  New Group Name12", 6, true)]   // Valid case
    [InlineData("13`520(#^(&@$_)@^", 7, true)]    // Valid case
    [InlineData(null, 5, false)]                  // Null group name
    [InlineData("", 5, false)]                    // Empty group name
    [InlineData(" ", 5,false)]                    // Whitespace group name
    public async Task UpdateGroup_ChangeGroupName_ShouldHandleDifferentCases_ReturnCorrectNameChangeResultOrException
        (string newChangedGroupName, int groupId, bool expectChange)
    {
        // Arrange
        var groupDto = new GroupDto
        {
            Id = groupId,
            Name = newChangedGroupName
        };
        
        if (expectChange)
        {
            // Act
            await _groupService.UpdateGroup(groupDto);

            // Assert
            var updatedGroup = await _dbContext.Groups.FindAsync(groupId);
            
            Assert.Equal(newChangedGroupName, updatedGroup!.Name);
        }
        else
        {
            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () =>
            {
                await _groupService.UpdateGroup(groupDto);
            });
        }
    }
    
    [Theory]
    [InlineData("SEE-22", 2, true)]                  // Valid case
    [InlineData("   SWE-22 ", 1, true)]              // Valid case
    [InlineData("()%$#* 3 _@)$#$ Name  ", 1, true)]  // Valid case
    [InlineData("SWE-41", default, false)]           // Null courseId
    [InlineData(null, 1,false)]                      // Null group name
    [InlineData("", 1, false)]                       // Empty group name
    [InlineData(" ", 1,false)]                       // Whitespace group name
    public async Task CreateGroup_ShouldHandleDifferentCases_ReturnCorrectGroupCreationResultOrException
        (string newGroupName, int courseId, bool expectCreate)
    {
        // Arrange
        var newGroupDto = new GroupDto
        {
            Name = newGroupName,
            CourseId = courseId
        };
        
        if (expectCreate)
        {
            // Act
            await _groupService.CreateGroup(newGroupDto);

            // Assert
            var createdGroup = await _dbContext.Groups.FirstOrDefaultAsync(g => g.Name == newGroupName);
            
            Assert.NotNull(createdGroup);
            Assert.Equal(newGroupName, createdGroup.Name);
        }
        else
        {
            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () => 
            {
                await _groupService.CreateGroup(newGroupDto);
            });
        }
    }
    
    [Theory]
    [InlineData(11, true)]        // Valid case
    [InlineData(12, true)]        // Valid case
    [InlineData(13, true)]        // Valid case and within course
    [InlineData(14, false)]       // Have students in it
    [InlineData(0, false)]        // Non-existent group
    [InlineData(default, false)]  // Non-existent group
    public async Task DeleteGroup_ShouldHandleDifferentCases_ReturnCorrectGroupDeletionResultOrException
        (int groupId, bool expectDelete)
    {
        // Arrange
        var groupDto = new GroupDto
        {
            Id = groupId
        };
        
        if (expectDelete)
        {
            // Act
            await _groupService.DeleteGroup(groupDto);

            // Assert
            var deletedGroup = await _dbContext.Groups.FindAsync(groupId);
            
            Assert.Null(deletedGroup);
        }
        else
        {
            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () =>
            {
                await _groupService.DeleteGroup(groupDto);
            });
        }
    }

    #endregion

    #region DbSetupAndDataSeeding
    
    private UniversityDbContext CreateAndSeedTestDb()
    {
        var options = new DbContextOptionsBuilder<UniversityDbContext>()
            .UseInMemoryDatabase(databaseName: "InMemory_UniversityServerDB_22")
            .Options;
        
        // Add seed data to the in-memory database for tests cases. 
        var dbContext = new UniversityDbContext(options);
        
        dbContext.Courses.AddRange(
            new() { Id = 1, Name = "System Engineer" },
            new() { Id = 2, Name = "Software Engineer" },
            new() { Id = 4, Name = "Data Analysis" },
            new() { Id = 5, Name = "Cyber Security" }
        );
        
        dbContext.Groups.AddRange(
            new() { Id = 1, Name = "SSE-11", CourseId = 1 },
            new() { Id = 2, Name = "CS-44", CourseId = 5 },
            new() { Id = 3, Name = "DA-12", CourseId = 4 },
            new() { Id = 4, Name = "DA-41", CourseId = 4 },
            new() { Id = 5, Name = "SWE-11" },
            new() { Id = 6, Name = "SWE-12" },
            new() { Id = 7, Name = "SWE-13" },
            new() { Id = 11, Name = "SWE-21" },
            new() { Id = 12, Name = " dwd 23 r3 g 59  !~" },
            new() { Id = 13, Name = "SWE-32", CourseId = 2 }
        );
        
        dbContext.Students.AddRange(
            new() { Id = 1, FirstName = "Oleg", LastName = "Kotlyar", GroupId =  1 },
            new() { Id = 2, FirstName = "Adam", LastName = "Kishinev", GroupId = 2 },
            new() { Id = 3, FirstName = "Sam", LastName = "Stone", GroupId = 2 }
        );
        
        dbContext.Students.Add(new() { Id = 20, FirstName = "Darya", LastName = "Lebovsky", GroupId = 14 });
        
        dbContext.SaveChangesAsync();

        return new UniversityDbContext(options);
    }
    
    private UniversityDbContext CreateEmptyTestDb()
    {
        var options = new DbContextOptionsBuilder<UniversityDbContext>()
            .UseInMemoryDatabase(databaseName: "InMemory_EmptyUniversityServerDB_22")
            .Options;
        
        return new UniversityDbContext(options);
    }

    #endregion
}