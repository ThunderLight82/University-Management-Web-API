using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using UniversityManagement.Application.EntitiesDto;
using UniversityManagement.Application.Services;
using UniversityManagement.Application.Services.Interfaces;
using UniversityManagement.Domain.Entities;
using UniversityManagement.Infrastructure;
using UniversityManagement.WebApi.AutoMapper;
using Xunit;

namespace UniversityManagement.UnitTests;

public class StudentServiceTests
{
    private readonly Mock<ILogger<StudentService>> _mockLoggerService;
    private readonly IMapper _testMapper;
    private readonly UniversityDbContext _dbContext;
    private readonly UniversityDbContext _emptyDbContext;
    private readonly IStudentService _studentService;

    public StudentServiceTests()
    {
        _mockLoggerService = new Mock<ILogger<StudentService>>();
        
        _testMapper = new MapperConfiguration(cfg => cfg
                .AddProfile(new EntitiesMapper()))
            .CreateMapper();
        
        _dbContext = CreateAndSeedTestDb();
        
        _studentService = new StudentService(_dbContext, _testMapper, _mockLoggerService.Object);
        
        //Separate empty DB for null or empty cases.  
        _emptyDbContext = CreateEmptyTestDb(); 
    }

    #region LogicTests

    [Fact]
    public async Task GetStudentsByGroupId_StudentsAndGroupExist_ReturnCorrectStudentsByGroupIdFromRepo()
    {
        // Arrange & Act
        var students = await _studentService.GetStudentsAllByGroupId(1);

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
        var students = await _studentService.GetStudentsAllByGroupId(2);

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
        var exception = await Assert.ThrowsAsync<Exception>(async () => await _studentService.GetStudentsAllByGroupId(4));
        Assert.Equal("There are no students for group with Id [4] in DB set.", exception.Message);
    }
    
    [Fact]
    public async Task GetStudentsByGroupId_NotDataFoundInDbOrNull_ThrowException()
    {
        // Arrange
        var studentService = new StudentService(_emptyDbContext, _testMapper, _mockLoggerService.Object);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(async () => await studentService.GetStudentsAllByGroupId(1));
        await Assert.ThrowsAsync<Exception>(async () => await studentService.GetStudentsAllByGroupId(0));
        await Assert.ThrowsAsync<Exception>(async () => await studentService.GetStudentsAllByGroupId(123));
        await Assert.ThrowsAsync<Exception>(async () => await studentService.GetStudentsAllByGroupId(-1));
    }
    
    [Theory]
    [InlineData("New First Name", 4, true)]           // Valid case
    [InlineData("  New First Name123", 5, true)]      // Valid case
    [InlineData("13  `520(#^(&@$_)@^  ", 6, true)]    // Valid case
    [InlineData(null, 4, false)]                      // Null first name
    [InlineData("", 4, false)]                        // Empty first name
    [InlineData(" ", 4, false)]                       // Whitespace first name
    public async Task ChangeStudentFirstName_ShouldHandleDifferentCases_ReturnCorrectNameChangeResultOrException
        (string newChangedFirstName, int studentId, bool expectChange)
    {
        // Arrange
        if (expectChange)
        {
            // Act
            await _studentService.ChangeStudentFirstName(newChangedFirstName, studentId);

            // Assert
            var updatedStudent = await _dbContext.Students.FindAsync(studentId);
            
            Assert.Equal(newChangedFirstName, updatedStudent!.FirstName);
        }
        else
        {
            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () =>
            {
                await _studentService.ChangeStudentFirstName(newChangedFirstName, studentId);
            });
        }
    }
    
    [Theory]
    [InlineData("New Last Name", 4, true)]            // Valid case
    [InlineData("  New Last Name123", 5, true)]       // Valid case
    [InlineData("13  `520(#^(&@$_)@^  ", 6, true)]    // Valid case
    [InlineData(null, 4, false)]                      // Null first name
    [InlineData("", 4, false)]                        // Empty first name
    [InlineData(" ", 4, false)]                       // Whitespace first name
    public async Task ChangeStudentLastName_ShouldHandleDifferentCases_ReturnCorrectNameChangeResultOrException
        (string newChangedLastName, int studentId, bool expectChange)
    {
        // Arrange
        if (expectChange)
        {
            // Act
            await _studentService.ChangeStudentLastName(newChangedLastName, studentId);

            // Assert
            var updatedStudent = await _dbContext.Students.FindAsync(studentId);
            
            Assert.Equal(newChangedLastName, updatedStudent!.LastName);
        }
        else
        {
            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () =>
            {
                await _studentService.ChangeStudentLastName(newChangedLastName, studentId);
            });
        }
    }
    
    [Theory]
    [InlineData("SomeName1", "SomeName2", true)]             // Valid case
    [InlineData("  Alex ", " Caterpillar ", true)]           // Valid case
    [InlineData(" ( )%$#* ", " dSome nameNam 3 !", true)]    // Valid case
    [InlineData(",", ".", true)]                             // Valid case
    [InlineData(null, "Joe", false)]                         // Null first name
    [InlineData("Joe", null, false)]                         // Null last name
    [InlineData("", "Owl", false)]                           // Empty first name
    [InlineData("Wolf", "", false)]                          // Empty last name
    [InlineData(" ", "Strangelove", false)]                  // Whitespace first name
    [InlineData("Snake", " ", false)]                        // Whitespace last name
    public async Task CreateStudent_ShouldHandleDifferentCases_ReturnCorrectStudentCreationResultOrException
        (string newFirstName, string newLastName, bool expectCreate)
    {
        // Arrange
        var newStudentDto = new StudentDto { FirstName = newFirstName, LastName = newLastName };

        if (expectCreate)
        {
            // Act
            await _studentService.CreateStudent(newStudentDto, newFirstName, newLastName);

            // Assert
            var createdStudent = await _dbContext.Students.FirstOrDefaultAsync(s =>
                s.FirstName == newFirstName && s.LastName == newLastName);
            
            Assert.NotNull(createdStudent);
            Assert.Equal(newFirstName, createdStudent.FirstName);
            Assert.Equal(newLastName, createdStudent.LastName);
        }
        else
        {
            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () =>
            {
                await _studentService.CreateStudent(newStudentDto, newFirstName, newLastName);
            });
        }
    }
    
    [Theory]
    [InlineData(11, true)]         // Valid case
    [InlineData(12, true)]         // Valid case
    [InlineData(13, true)]         // Valid case and within group
    [InlineData(99, false)]        // Non-existent student
    [InlineData(0, false)]         // Non-existent student
    [InlineData(default, false)]   // Non-existent student
    public async Task DeleteStudent_ShouldHandleDifferentCases_ReturnCorrectStudentDeletionResultOrException
        (int studentId, bool expectDelete)
    {
        // Arrange & Act & Assert
        if (expectDelete)
        {
            // Act
            await _studentService.DeleteStudent(studentId);

            // Assert
            var deletedStudent = await _dbContext.Students.FindAsync(studentId);
            
            Assert.Null(deletedStudent);
        }
        else
        {
            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () =>
            {
                await _studentService.DeleteStudent(studentId);
            });
        }
    }
    
    [Theory]
    [InlineData(21, 11, true)]          // Valid case
    [InlineData(22, 11, true)]          // Valid case
    [InlineData(23, 12, true)]          // Valid case
    [InlineData(1, 3, false)]           // Student already in a group
    [InlineData(999, 1, false)]         // Non-existent student
    [InlineData(999, 999, false)]       // Non-existent group and student
    [InlineData(0, 1, false)]           // Non-existent student and valid group
    [InlineData(default, 1, false)]     // Non-existent student and valid group
    [InlineData(24, default, false)]    // Student exist but non valid group
    public async Task AddStudentToGroup_ShouldHandleDifferentCases_ReturnCorrectAddingResultOrException
        (int studentId, int groupId, bool expectAdd)
    {
        // Arrange & Act & Assert
        if (expectAdd)
        {
            // Act
            await _studentService.AddStudentToGroup(studentId, groupId);

            // Assert
            var updatedStudent = await _dbContext.Students.FindAsync(studentId);
            
            Assert.Equal(groupId, updatedStudent!.GroupId);
        }
        else
        {
            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () =>
            {
                await _studentService.AddStudentToGroup(studentId, groupId);
            });
        }
    }
    
    [Theory]
    [InlineData(31, true)]            // Valid case
    [InlineData(32, true)]            // Valid case
    [InlineData(24, true)]            // Student is not in any group
    [InlineData(999, false)]          // Non-existent student
    [InlineData(0, false)]            // Non-existent student
    [InlineData(default, false)]      // Non-existent student
    public async Task RemoveStudentFromGroup_ShouldHandleDifferentCases_ReturnCorrectDeletionResultOrException
        (int studentId, bool expectRemove)
    {
        // Arrange & Act & Assert
        if (expectRemove)
        {
            // Act
            await _studentService.RemoveStudentFromGroup(studentId);

            // Assert
            var updatedStudent = await _dbContext.Students.FindAsync(studentId);
            
            Assert.NotNull(updatedStudent);
            Assert.Equal(default, updatedStudent.GroupId);
        }
        else
        {
            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () =>
            {
                await _studentService.RemoveStudentFromGroup(studentId);
            });
        }
    }

    #endregion

    #region DbSetupAndDataSeeding

    private UniversityDbContext CreateAndSeedTestDb()
    {
        var options = new DbContextOptionsBuilder<UniversityDbContext>()
            .UseInMemoryDatabase(databaseName: "InMemory_UniversityServerDB_23")
            .Options;

        // Add seed data to the in-memory database for tests cases. 
        var dbContext = new UniversityDbContext(options);
        
        dbContext.Courses.Add(
            new Course { Id = 1, Name = "System Engineer" }
        );
        
        dbContext.Groups.AddRange(
            new() { Id = 1, Name = "SSE-11", CourseId =  1 },
            new() { Id = 2, Name = "SSE-32", CourseId = 1 },
            new() { Id = 3, Name = "SSE-41", CourseId = 1 },
            new() { Id = 4, Name = "SSE-44", CourseId = 1 }
        );
        
        dbContext.Students.AddRange(
            new() { Id = 1, FirstName = "Oleg", LastName = "Kotlyar", GroupId =  1 },
            new() { Id = 2, FirstName = "Adam", LastName = "Kishinev", GroupId = 2 },
            new() { Id = 3, FirstName = "Sam", LastName = "Stone", GroupId = 2 }, 
            new() { Id = 4, FirstName = "Samsa", LastName = "Salsa" }, 
            new() { Id = 5, FirstName = "Dotto", LastName = "Kvas" }, 
            new() { Id = 6, FirstName = "Reighnore", LastName = "Daato" },
            new() { Id = 11, FirstName = "Wowe", LastName = "Vulpo" }, 
            new() { Id = 12, FirstName = "Fenrir", LastName = "Northern" }, 
            new() { Id = 13, FirstName = "Villa", LastName = "Vi", GroupId = 3 },
            new() { Id = 21, FirstName = "Eye", LastName = "Eagle" }, 
            new() { Id = 22, FirstName = "_0_", LastName = "_1_" },
            new() { Id = 23, FirstName = "Empty", LastName = "NotNull" },
            new() { Id = 24, FirstName = "Yogi", LastName = "Dani" },
            new() { Id = 31, FirstName = "Three", LastName = "Green", GroupId = 3 },
            new() { Id = 32, FirstName = "Purple", LastName = "Dark", GroupId = 3 }
        );
        
        dbContext.SaveChangesAsync();
        
        return new UniversityDbContext(options);
    }
    
    private UniversityDbContext CreateEmptyTestDb()
    {
        var options = new DbContextOptionsBuilder<UniversityDbContext>()
            .UseInMemoryDatabase(databaseName: "InMemory_EmptyUniversityServerDB_23")
            .Options;

        return new UniversityDbContext(options);
    }

    #endregion
}