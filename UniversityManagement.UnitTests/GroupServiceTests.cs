using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using UniversityManagement.Application.EntitiesDto;
using UniversityManagement.Application.Services;
using UniversityManagement.Application.Services.Interfaces;
using UniversityManagement.Infrastructure;
using UniversityManagement.WebApi.AutoMapper;
using Xunit;

namespace UniversityManagement.UnitTests;

public class GroupServiceTests
{
    private readonly Mock<ILogger<GroupService>> _mockLoggerService;
    private readonly IMapper _testMapper;
    private readonly UniversityDbContext _dbContext;
    private readonly UniversityDbContext _emptyDbContext;
    private readonly IGroupService _groupService;

    public GroupServiceTests()
    {
        _mockLoggerService = new Mock<ILogger<GroupService>>();
        
        _testMapper = new MapperConfiguration(cfg => cfg
                .AddProfile(new EntitiesMapper()))
            .CreateMapper();

        _dbContext = CreateAndSeedTestDb();
        
        _groupService = new GroupService(_dbContext, _testMapper, _mockLoggerService.Object);

        //Separate empty DB for some null or empty cases.  
        _emptyDbContext = CreateEmptyTestDb();
    }

    #region LogicTest

    [Fact]
    public async Task GetGroupsByCourseId_GroupsAndCourseExist_ReturnCorrectGroupsByCourseIdFromRepo()
    {
        // Arrange & Act
        var groups = await _groupService.GetGroupsAllByCourseId(2);

        // Assert
        Assert.NotNull(groups);
        Assert.Single(groups);
        Assert.Contains(groups, g => g.Name == "SEE-22");
    }
    
    [Fact]
    public async Task GetGroupsByCourseId_GroupsAndCourseExist_ReturnAnotherCorrectGroupsByCourseIdFromRepo()
    { 
        // Arrange & Act
        var groups = await _groupService.GetGroupsAllByCourseId(4);

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
            await Assert.ThrowsAsync<Exception>(async () => await _groupService.GetGroupsAllByCourseId(5));
        
        Assert.Equal("There are no groups for course with Id [5] in DB set.", exception.Message);
    }
    
    [Fact]
    public async Task GetGroupsByCourseId_NotDataFoundInDbOrNull_ThrowException()
    { 
        // Arrange
        var groupService = new GroupService(_emptyDbContext, _testMapper, _mockLoggerService.Object);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(async () => await groupService.GetGroupsAllByCourseId(1));
        await Assert.ThrowsAsync<Exception>(async () => await groupService.GetGroupsAllByCourseId(0));
        await Assert.ThrowsAsync<Exception>(async () => await groupService.GetGroupsAllByCourseId(123));
        await Assert.ThrowsAsync<Exception>(async () => await groupService.GetGroupsAllByCourseId(-1));
    }
    
    [Theory]
    [InlineData("New Group Name", 5, true)]       // Valid case
    [InlineData("  New Group Name12", 6, true)]   // Valid case
    [InlineData("13`520(#^(&@$_)@^", 7, true)]    // Valid case
    [InlineData(null, 5, false)]                  // Null group name
    [InlineData("", 5, false)]                    // Empty group name
    [InlineData(" ", 5,false)]                    // Whitespace group name
    public async Task ChangeGroupName_ShouldHandleDifferentCases_ReturnCorrectNameChangeResultOrException
        (string newChangedGroupName, int groupId, bool expectChange)
    {
        // Arrange
        if (expectChange)
        {
            // Act
            await _groupService.ChangeGroupName(newChangedGroupName, groupId);

            // Assert
            var updatedGroup = await _dbContext.Groups.FindAsync(groupId);
            
            Assert.Equal(newChangedGroupName, updatedGroup!.Name);
        }
        else
        {
            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () =>
            {
                await _groupService.ChangeGroupName(newChangedGroupName, groupId);
            });
        }
    }
    
    [Theory]
    [InlineData("SEE-22", 2, true)]                  // Valid case
    [InlineData("   SWE-22 ", 1, true)]              // Valid case
    [InlineData("()%$#* 3 _@)$#$ Name  ", 1, true)]  // Valid case
    [InlineData("SWE-41", default, false)]            // Null courseId
    [InlineData(null, 1,false)]                      // Null group name
    [InlineData("", 1, false)]                       // Empty group name
    [InlineData(" ", 1,false)]                       // Whitespace group name
    public async Task CreateGroup_ShouldHandleDifferentCases_ReturnCorrectGroupCreationResultOrException
        (string newGroupName, int courseId, bool expectCreate)
    {
        // Arrange
        var newGroupDto = new GroupDto { Name = newGroupName };
        
        if (expectCreate)
        {
            // Act
            await _groupService.CreateGroup(newGroupDto, newGroupName, courseId);

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
                await _groupService.CreateGroup(newGroupDto, newGroupName, courseId);
            });
        }
    }
    
    [Theory]
    [InlineData(11, true)]        // Valid case
    [InlineData(12, true)]        // Valid case
    [InlineData(13, true)]        // Valid case and within course
    [InlineData(14, false)]       // Have students in it
    [InlineData(99, false)]       // Non-existent group
    [InlineData(0, false)]        // Non-existent group
    [InlineData(default, false)]  // Non-existent group
    public async Task DeleteGroup_ShouldHandleDifferentCases_ReturnCorrectGroupDeletionResultOrException
        (int groupId, bool expectDelete)
    {
        // Arrange & Act & Assert
        if (expectDelete)
        {
            // Act
            await _groupService.DeleteGroup(groupId);

            // Assert
            var deletedGroup = await _dbContext.Groups.FindAsync(groupId);
            
            Assert.Null(deletedGroup);
        }
        else
        {
            await Assert.ThrowsAsync<Exception>(async () =>
            {
                await _groupService.DeleteGroup(groupId);
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
            new() { Id = 3, Name = "DA-12", CourseId = 4 },
            new() { Id = 4, Name = "DA-41", CourseId = 4 },
            new() { Id = 5, Name = "SWE-11" },
            new() { Id = 6, Name = "SWE-12" },
            new() { Id = 7, Name = "SWE-13" },
            new() { Id = 11, Name = "SWE-21" },
            new() { Id = 12, Name = " dwd 23 r3 g 59  !~" },
            new() { Id = 13, Name = "SWE-32", CourseId = 2 }
        );
        
        dbContext.Students.Add(new() { Id = 1, FirstName = "Darya", LastName = "Lebovsky", GroupId = 14 });
        
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