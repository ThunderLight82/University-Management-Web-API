using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using UniversityManagement.Application.Services;
using UniversityManagement.Application.Services.Interfaces;
using UniversityManagement.Application.Validations;
using UniversityManagement.Domain.Entities;
using UniversityManagement.DataAccess;
using UniversityManagement.DTO.EntitiesDto;
using UniversityManagement.WebApi.AutoMapper;
using Xunit;

namespace UniversityManagement.UnitTests;

public class StudentServiceTests
{
    private readonly Mock<ILogger<StudentService>> _mockLoggerService;
    private readonly Mock<ILogger<ValidationService>> _mockLoggerValidationService;
    private readonly IMapper _testMapper;
    private readonly UniversityDbContext _dbContext;
    private readonly UniversityDbContext _emptyDbContext;
    private readonly IValidationService _validationService;
    private readonly IStudentService _studentService;

    public StudentServiceTests()
    {
        _mockLoggerService = new Mock<ILogger<StudentService>>();
        _mockLoggerValidationService = new Mock<ILogger<ValidationService>>();
        
        _testMapper = new MapperConfiguration(cfg => cfg
                .AddProfile(new EntitiesMapper()))
            .CreateMapper();
        
        _dbContext = CreateAndSeedTestDb();

        _validationService = new ValidationService(_mockLoggerValidationService.Object);
        
        _studentService = new StudentService(_dbContext, _testMapper, _mockLoggerService.Object, _validationService);
        
        //Separate empty DB for null or empty cases.  
        _emptyDbContext = CreateEmptyTestDb(); 
    }

    #region LogicTests
    
    [Theory]
    [InlineData("New First Name", 4, true)]           // Valid case
    [InlineData("  New First Name123", 5, true)]      // Valid case
    [InlineData("13  `520(#^(&@$_)@^  ", 6, true)]    // Valid case
    [InlineData(null, 4, false)]                      // Null first name
    [InlineData("", 4, false)]                        // Empty first name
    [InlineData(" ", 4, false)]                       // Whitespace first name
    public async Task UpdateStudent_ChangeStudentFirstName_ShouldHandleDifferentCases_ReturnCorrectNameChangeResultOrException
        (string newChangedFirstName, int studentId, bool expectChange)
    {
        // Arrange
        var studentDto = new StudentDto
        {
            Id = studentId,
            FirstName = newChangedFirstName,
            LastName = "TestLastName"
        };
        
        if (expectChange)
        {
            // Act
            await _studentService.UpdateStudent(studentDto);
            
            var updatedStudentDto = await _dbContext.Students.FindAsync(studentId);
            
            // Assert
            Assert.Equal(newChangedFirstName, updatedStudentDto!.FirstName);
        }
        else
        {
            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () =>
            {
                await _studentService.UpdateStudent(studentDto);
            });
        }
    }
    
    [Theory]
    [InlineData("New Last Name", 4, true)]            // Valid case
    [InlineData("  New Last Name123", 5, true)]       // Valid case
    [InlineData("13  `520(#^(&@$_)@^  ", 6, true)]    // Valid case
    [InlineData(null, 4, false)]                      // Null last name
    [InlineData("", 4, false)]                        // Empty last name
    [InlineData(" ", 4, false)]                       // Whitespace last name
    public async Task UpdateStudent_ChangeStudentLastName_ShouldHandleDifferentCases_ReturnCorrectNameChangeResultOrException
        (string newChangedLastName, int studentId, bool expectChange)
    {
        // Arrange
        var studentDto = new StudentDto
        {
            Id = studentId,
            LastName = newChangedLastName,
            FirstName = "TestFirstName"
        };
        
        if (expectChange)
        {
            // Act
            await _studentService.UpdateStudent(studentDto);

            // Assert
            var updatedStudent = await _dbContext.Students.FindAsync(studentId);
            
            Assert.Equal(newChangedLastName, updatedStudent!.LastName);
        }
        else
        {
            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () =>
            {
                await _studentService.UpdateStudent(studentDto);
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
        var newStudentDto = new StudentDto
        {
            FirstName = newFirstName, 
            LastName = newLastName
        };

        if (expectCreate)
        {
            // Act
            await _studentService.CreateStudent(newStudentDto);

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
                await _studentService.CreateStudent(newStudentDto);
            });
        }
    }
    
    [Theory]
    [InlineData(11, true)]         // Valid case
    [InlineData(12, true)]         // Valid case
    [InlineData(13, true)]         // Valid case and within group
    [InlineData(0, false)]         // Non-existent student
    [InlineData(default, false)]   // Non-existent student
    public async Task DeleteStudent_ShouldHandleDifferentCases_ReturnCorrectStudentDeletionResultOrException
        (int studentId, bool expectDelete)
    {
        // Arrange
        var studentDto = new StudentDto
        {
            Id = studentId,
        };
        
        if (expectDelete)
        {
            // Act
            await _studentService.DeleteStudent(studentDto);

            // Assert
            var deletedStudent = await _dbContext.Students.FindAsync(studentId);
            
            Assert.Null(deletedStudent);
        }
        else
        {
            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () =>
            {
                await _studentService.DeleteStudent(studentDto);
            });
        }
    }
    
    [Theory]
    [InlineData(21, 11, true)]          // Valid case
    [InlineData(22, 11, true)]          // Valid case
    [InlineData(23, 22, true)]          // Valid case
    [InlineData(1, 3, false)]           // Student already in a group
    [InlineData(0, 1, false)]           // Non-existent student and valid group
    [InlineData(default, 1, false)]     // Non-existent student and valid group
    public async Task AddStudentToGroup_ShouldHandleDifferentCases_ReturnCorrectAddingResultOrException
        (int studentId, int groupId, bool expectAdd)
    {
        // Arrange
        var studentDto = new StudentDto
        {
            Id = studentId,
            GroupId = groupId
        };
        
        if (expectAdd)
        {
            // Act
            await _studentService.AddStudentToGroup(studentDto);

            // Assert
            var updatedStudent = await _dbContext.Students.FindAsync(studentId);
            
            Assert.Equal(groupId, updatedStudent!.GroupId);
        }
        else
        {
            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () =>
            {
                await _studentService.AddStudentToGroup(studentDto);
            });
        }
    }
    
    [Theory]
    [InlineData(31, true)]            // Valid case
    [InlineData(32, true)]            // Valid case
    [InlineData(24, true)]            // Student is not in any group
    [InlineData(0, false)]            // Non-existent student
    [InlineData(default, false)]      // Non-existent student
    public async Task RemoveStudentFromGroup_ShouldHandleDifferentCases_ReturnCorrectDeletionResultOrException
        (int studentId, bool expectRemove)
    {
        // Arrange
        var studentDto = new StudentDto
        {
            Id = studentId
        };
        
        if (expectRemove)
        {
            // Act
            await _studentService.RemoveStudentFromGroup(studentDto);

            // Assert
            var updatedStudent = await _dbContext.Students.FindAsync(studentId);
            
            Assert.NotNull(updatedStudent);
            Assert.Null(updatedStudent.GroupId);
        }
        else
        {
            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () =>
            {
                await _studentService.RemoveStudentFromGroup(studentDto);
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
            new() { Id = 4, Name = "SSE-44", CourseId = 1 },
            new() { Id = 11, Name = "SSE-441", CourseId = 1 },
            new() { Id = 22, Name = "SSE-444", CourseId = 1 }
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