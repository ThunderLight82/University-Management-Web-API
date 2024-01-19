using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UniversityManagement.Application.EntitiesDto;
using UniversityManagement.Application.Services.Interfaces;
using UniversityManagement.Domain.Entities;
using UniversityManagement.DataAccess;

namespace UniversityManagement.Application.Services;

public class GroupService : BaseService<Group>, IGroupService
{
    private readonly IMapper _mapper;
    private readonly ILogger<GroupService> _logger;
    private readonly IValidationService _validationService;

    public GroupService(UniversityDbContext dbContext, IMapper mapper, ILogger<GroupService> logger, IValidationService validationService)
        : base(dbContext)
    {
        _mapper = mapper;
        _logger = logger;
        _validationService = validationService;
    }
    
    public async Task<GroupDto> GetGroupById(int groupId)
    {
        _validationService.ValidateGetGroupsById(groupId);
        
        var group = await GetById(groupId);
        
        return _mapper.Map<GroupDto>(group);
    }

    public async Task<IEnumerable<GroupDto>> GetGroups()
    {
        var groups = await GetAll();
        
        _validationService.ValidateGetGroups(groups);
        
        return _mapper.Map<IEnumerable<GroupDto>>(groups);
    }
    
    public async Task<IEnumerable<StudentDto>> GetStudentsByGroupId(int groupId)
    {
        _validationService.ValidateGetStudentsByGroupId(groupId);
        
        var group = await _dbContext.Groups
            .Where(g => g.Id == groupId)
            .Include(g => g.Students)
            .FirstOrDefaultAsync();
        
        if (group!.Students.Count == 0)
        {
            _logger.LogError($"Error in GroupService - GetStudentsListByGroupId while fetching students by group Id [{groupId}]");
            throw new Exception($"There are no students for group with Id [{groupId}] in DB set.");
        }

        return _mapper.Map<IEnumerable<StudentDto>>(group.Students);
    }
    
    public async Task<GroupDto> UpdateGroup(GroupDto groupDto)
    {
        _validationService.ValidateUpdateGroup(groupDto);
        
        var group = await GetById(groupDto.Id);
        
        group.Name = groupDto.Name;
        
        await Update(group);
        _logger.LogInformation($"Group with Id [{groupDto.Id}] name changed successfully. New name is '{groupDto.Name}'.");
        
        return _mapper.Map<GroupDto>(group);
    }
    
    public async Task<GroupDto> CreateGroup(GroupDto newGroupDto)
    {
        _validationService.ValidateCreateGroup(newGroupDto);
        
        var existingGroup = await _dbContext.Groups.FirstOrDefaultAsync(g => g.Name == newGroupDto.Name);

        if (existingGroup != null)
        {
            _logger.LogError($"Error in GroupService - CreateGroup group with name [{newGroupDto.Name}] already exists.");
            throw new Exception($"Group with name '{newGroupDto.Name}' already exists.");
        }
        
        var groupEntity = _mapper.Map<Group>(newGroupDto);
        
        groupEntity.Name = newGroupDto.Name;
        groupEntity.CourseId = newGroupDto.CourseId;
        
        await Add(groupEntity);
        _logger.LogInformation($"New group with name [{newGroupDto.Name}] is successfully created within course with Id:[{newGroupDto.CourseId}]");
        
        return _mapper.Map<GroupDto>(groupEntity);
    }

    public async Task DeleteGroup(GroupDto groupDto)
    {
        _validationService.ValidateDeleteGroup(groupDto);
        
        var group = await GetById(groupDto.Id);
        
        if (_dbContext.Students.Any(s => s.GroupId == groupDto.Id))
        {
            _logger.LogError($"Error in GroupService - DeleteGroup: Group with Id [{groupDto.Id}] cannot be deleted as it has associated students.");
            throw new Exception($"Group with Id [{groupDto.Id}] cannot be deleted as it has associated students.");
        }
        
        await Delete(group);
        _logger.LogInformation($"Group with Id [{groupDto.Id}] was successfully deleted");
    }
}