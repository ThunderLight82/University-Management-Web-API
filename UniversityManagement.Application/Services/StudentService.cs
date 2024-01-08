using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UniversityManagement.Application.EntitiesDto;
using UniversityManagement.Application.Services.Interfaces;
using UniversityManagement.Domain.Entities;
using UniversityManagement.Infrastructure;

namespace UniversityManagement.Application.Services;

public class StudentService : BaseService<Student>, IStudentService
{
    private readonly IMapper _mapper;
    private readonly ILogger<StudentService> _logger;

    public StudentService(UniversityDbContext dbContext, IMapper mapper, ILogger<StudentService> logger) 
        : base(dbContext)
    {
        _mapper = mapper;
        _logger = logger;
    }
    
    public async Task<StudentDto> GetStudentById(int studentId)
    {
        var student = await GetById(studentId);

        if (student == null)
        {
            _logger.LogError($"Error in StudentService - GetStudentById while fetching student by Id [{studentId}]");
            throw new Exception($"There is no student with Id [{studentId}] in DB set.");
        }

        return _mapper.Map<StudentDto>(student);
    }

    public async Task<IEnumerable<StudentDto>> GetStudentsAll()
    {
        var students = await GetAll();

        if (students == null)
        {
            _logger.LogError("Error in StudentService - GetStudentAll while fetching all students.");
            throw new Exception("Failed to retrieve student list from DB set.");
        }

        return _mapper.Map<IEnumerable<StudentDto>>(students);
    }
    
    public async Task<IEnumerable<StudentDto>> GetStudentsAllWithGroupName()
    {
        var studentGroupName = await _dbContext.Students
            .Include(s => s.Group)
            .ToListAsync();

        if (studentGroupName == null)
        {
            _logger.LogError("Error in StudentService - GetStudentsAllWithGroupName while fetching all students with their group names.");
            throw new Exception("Failed to retrieve students group names from DB set.");
        }

        return _mapper.Map<IEnumerable<StudentDto>>(studentGroupName);
    }
    
    public async Task<IEnumerable<StudentDto>> GetStudentsAllByGroupId(int groupId)
    {
        var group = await _dbContext.Groups
            .Where(g => g.Id == groupId)
            .Include(g => g.Students)
            .FirstOrDefaultAsync();
        
        if (group == null)
        {
            _logger.LogError($"Error in StudentService - GetStudentsListByGroupId while fetching students by group Id [{groupId}]");
            throw new Exception($"There is no group with Id [{groupId}] in DB set.");
        }
        
        if (group.Students.Count == 0)
        {
            _logger.LogError($"Error in StudentService - GetStudentsListByGroupId while fetching students by group Id [{groupId}]");
            throw new Exception($"There are no students for group with Id [{groupId}] in DB set.");
        }

        return _mapper.Map<IEnumerable<StudentDto>>(group.Students);
    }
    
    public async Task<StudentDto> ChangeStudentFirstName(string newChangedFirstName, int studentId)
    {
        var student = await GetById(studentId);

        if (student == null)
        {
            _logger.LogError($"Error in StudentService - ChangeFirstName while fetching student with Id [{studentId}]");
            throw new Exception($"There is no student with Id [{studentId}] in DB set.");
        }

        student.FirstName = newChangedFirstName;

        if (string.IsNullOrWhiteSpace(newChangedFirstName))
        {
            _logger.LogError($"Error in StudentService - ChangeFirstName: new first name is null or empty for student with Id [{studentId}]");
            throw new Exception($"New first name is null or empty for student with Id [{studentId}]");
        }

        await Update(student);
        
        _logger.LogInformation($"Student with id [{studentId}] first name changed successfully. New first name is '{newChangedFirstName}'");
        return _mapper.Map<StudentDto>(student);
    }
    
    public async Task<StudentDto> ChangeStudentLastName(string newChangedLastName, int studentId)
    {
        var student = await GetById(studentId);

        if (student == null)
        {
            _logger.LogError($"Error in StudentService - ChangeLastName while fetching student with Id [{studentId}]");
            throw new Exception($"There is no student with Id [{studentId}] in DB set.");
        }

        student.LastName = newChangedLastName;

        if (string.IsNullOrWhiteSpace(newChangedLastName))
        {
            _logger.LogError($"Error in StudentService - ChangeLastName: new last name is null or empty for student with Id [{studentId}]");
            throw new Exception($"New last name is null or empty for student with Id [{studentId}]");
        }

        await Update(student);
        
        _logger.LogInformation($"Student with id [{studentId}] last name changed successfully. New last name is '{newChangedLastName}'");
        return _mapper.Map<StudentDto>(student);
    }
    
    public async Task<StudentDto> CreateStudent(StudentDto newStudentDto, string newStudentFirstName, string newStudentLastName)
    {
        if (string.IsNullOrWhiteSpace(newStudentFirstName) || string.IsNullOrWhiteSpace(newStudentLastName))
        {
            _logger.LogError("Error in StudentService - CreateStudent new created student name is null or empty.");
            throw new Exception("New created student name is null or empty in First/Last name fields.");
        }
        
        var studentEntity = _mapper.Map<Student>(newStudentDto);
        
        studentEntity.FirstName = newStudentFirstName ;
        studentEntity.LastName = newStudentLastName;
        studentEntity.GroupId = default;

        await Add(studentEntity);
        
        _logger.LogInformation($"Student '{newStudentFirstName} " + $"{newStudentLastName}' created successfully.");
        return _mapper.Map<StudentDto>(studentEntity);
    }
    
    public async Task DeleteStudent(int studentId)
    {
        var student = await GetById(studentId);

        if (student == null)
        {
            _logger.LogError($"Error in StudentService - DeleteStudent while fetching student with Id [{studentId}]");
            throw new Exception($"There is no student with Id [{studentId}] in DB set.");
        }
        
        _logger.LogInformation($"Student with id [{studentId}] was successfully deleted.");
        await Delete(studentId);
    }
    
    public async Task AddStudentToGroup(int studentId, int groupId)
    {
        var student = await GetById(studentId);

        if (student == null)
        {
            _logger.LogError($"Error in StudentService - AddStudentToGroup while fetching student with Id [{studentId}]");
            throw new Exception($"There is no student with Id [{studentId}] in DB set.");
        }
        
        if (groupId == default)
        {
            _logger.LogError($"Error in StudentService - AddStudentToGroup while fetching group with Id [{studentId}]");
            throw new Exception($"There is no group with Id [{groupId}] in DB set.");
        }
        
        if (student.GroupId != default)
        {
            _logger.LogError($"Error in StudentService - AddStudentToGroup student with Id [{studentId}] is already in a group with Id [{student.GroupId}]");
            throw new Exception($"Student with Id [{studentId}] is already in a group with Id[{student.GroupId}]");
        }

        student.GroupId = groupId;
        
        _logger.LogInformation($"Student with Id [{studentId}] was successfully added to group with Id [{groupId}].");
        await Update(student);
    }
    
    public async Task RemoveStudentFromGroup(int studentId)
    {
        var student = await GetById(studentId);

        if (student == null)
        {
            _logger.LogError($"Error in StudentService - RemoveStudentFromGroup while fetching student with Id [{studentId}]");
            throw new Exception($"There is no student with Id [{studentId}] in DB set.");
        }
        
        student.GroupId = default;
        
        _logger.LogInformation($"Student with Id [{studentId}] was successfully removed from group.");
        await Update(student);
    }
}