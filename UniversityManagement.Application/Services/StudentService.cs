using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UniversityManagement.Application.EntitiesDto;
using UniversityManagement.Application.Services.Interfaces;
using UniversityManagement.Domain.Entities;
using UniversityManagement.DataAccess;

namespace UniversityManagement.Application.Services;

public class StudentService : BaseService<Student>, IStudentService
{
    private readonly IMapper _mapper;
    private readonly ILogger<StudentService> _logger;
    private readonly IValidationService _validationService;

    public StudentService(UniversityDbContext dbContext, IMapper mapper, ILogger<StudentService> logger, IValidationService validationService) 
        : base(dbContext)
    {
        _mapper = mapper;
        _logger = logger;
        _validationService = validationService;
    }
    
    public async Task<StudentDto> GetStudentById(int studentId)
    {
        _validationService.ValidateGetStudentById(studentId);
        
        var student = await GetById(studentId);
        
        return _mapper.Map<StudentDto>(student);
    }

    public async Task<IEnumerable<StudentDto>> GetStudents()
    {
        var students = await _dbContext.Students
            .Include(s => s.Group)
            .ToListAsync();
        
        _validationService.ValidateGetStudents(students);
        
        return _mapper.Map<IEnumerable<StudentDto>>(students);
    }

    public async Task<StudentDto> UpdateStudent(StudentDto studentDto)
    {
        _validationService.ValidateUpdateStudent(studentDto);
        
        var student = await GetById(studentDto.Id);
        
        student.FirstName = studentDto.FirstName;
        student.LastName = studentDto.LastName;
        
        await Update(student);
        _logger.LogInformation($"Student with id [{studentDto.Id}] first and/or last name changed successfully.");
        
        return _mapper.Map<StudentDto>(student);
    }
    
    public async Task<StudentDto> CreateStudent(StudentDto newStudentDto)
    {
        _validationService.ValidateCreateStudent(newStudentDto);
        
        var studentEntity = _mapper.Map<Student>(newStudentDto);
        
        studentEntity.FirstName = newStudentDto.FirstName;
        studentEntity.LastName = newStudentDto.LastName;
        studentEntity.GroupId = default;

        await Add(studentEntity);
        _logger.LogInformation($"Student '{newStudentDto.FirstName} " + $"{newStudentDto.LastName}' created successfully.");
        
        return _mapper.Map<StudentDto>(studentEntity);
    }
    
    public async Task DeleteStudent(StudentDto studentDto)
    {
        _validationService.ValidateDeleteStudent(studentDto);
        
        var student = await GetById(studentDto.Id);
        
        await Delete(student);
        _logger.LogInformation($"Student with id [{studentDto.Id}] was successfully deleted.");
    }
    
    public async Task AddStudentToGroup(StudentDto studentDto)
    {
        _validationService.ValidateStudentGroupOperation(studentDto);
        
        var student = await GetById(studentDto.Id);
        
        if (student.GroupId != default)
        {
            _logger.LogError($"Error in StudentService - AddStudentToGroup student with Id [{student.Id}] is already in a group with Id [{student.GroupId}]");
            throw new Exception($"Student with Id [{student.Id}] is already in a group with Id[{student.GroupId}]");
        }
        
        student.GroupId = studentDto.GroupId;
        
        await Update(student);
        _logger.LogInformation($"Student with Id [{studentDto.Id}] was successfully added to group with Id [{studentDto.GroupId}].");
    }
    
    public async Task RemoveStudentFromGroup(StudentDto studentDto)
    {
        _validationService.ValidateStudentGroupOperation(studentDto);
        
        var student = await GetById(studentDto.Id);
        
        student.GroupId = default;
        
        await Update(student);
        _logger.LogInformation($"Student with Id [{studentDto.Id}] was successfully removed from group.");
    }
}