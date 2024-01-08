using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UniversityManagement.Application.EntitiesDto;
using UniversityManagement.Application.Services.Interfaces;
using UniversityManagement.Domain.Entities;
using UniversityManagement.Infrastructure;

namespace UniversityManagement.Application.Services;

public class GroupService : BaseService<Group>, IGroupService
{
    private readonly IMapper _mapper;
    private readonly ILogger<GroupService> _logger;

    public GroupService(UniversityDbContext dbContext, IMapper mapper, ILogger<GroupService> logger)
        : base(dbContext)
    {
        _mapper = mapper;
        _logger = logger;
    }
    
    public async Task<GroupDto> GetGroupById(int groupId)
    {
        var group = await GetById(groupId);

        if (group != null)
        {
            return _mapper.Map<GroupDto>(group);
        }

        _logger.LogError($"Error in GroupService - GetGroupById while fetching group by Id [{groupId}]");
        throw new Exception($"There is no group with Id [{groupId}] in DB set.");
    }

    public async Task<IEnumerable<GroupDto>> GetGroupsAll()
    {
        var groups = await GetAll();

        if (groups != null)
        {
            return _mapper.Map<IEnumerable<GroupDto>>(groups);
        }
        
        _logger.LogError("Error in GroupService - GetGroupsAll while fetching all group.");
        throw new Exception("Failed to retrieve group list from DB set.");
    }
    
    public async Task<IEnumerable<GroupDto>> GetGroupsAllByCourseId(int courseId)
    {
        var course = await _dbContext.Courses
            .Where(c => c.Id == courseId)
            .Include(c => c.Groups)
            .FirstOrDefaultAsync();

        if (course == null)
        {
            _logger.LogError($"Error in GroupService - GetGroupsListByCourseId while fetching groups by course Id [{courseId}]");
            throw new Exception($"There is no course with Id [{courseId}] in DB set.");
        }

        if (course.Groups.Count == 0)
        {
            _logger.LogError($"Error in GroupService - GetGroupsListByCourseId while fetching groups by course Id [{courseId}]");
            throw new Exception($"There are no groups for course with Id [{courseId}] in DB set.");
        }
        
        return _mapper.Map<List<GroupDto>>(course.Groups);
    }
    
    public async Task<GroupDto> ChangeGroupName(string newChangedGroupName, int groupId)
    {
        var group = await GetById(groupId);
        
        if (group == null)
        {
            _logger.LogError($"Error in GroupService - ChangeGroupName while fetching group with Id [{groupId}]");
            throw new Exception($"There is no group with Id [{groupId}] in DB set.");
        }
        
        group.Name = newChangedGroupName;
        
        if (string.IsNullOrWhiteSpace(newChangedGroupName))
        {
            _logger.LogError($"Error in GroupService - ChangeGroupName new group name is null or empty for group with Id [{groupId}]");
            throw new Exception($"New group name is null or empty for group with Id [{groupId}]");
        }
        
        await Update(group);
        
        _logger.LogInformation($"Group with Id [{groupId}] name changed successfully. New name is '{newChangedGroupName}'.");
        return _mapper.Map<GroupDto>(group);
    }
    
    public async Task<GroupDto> CreateGroup(GroupDto newGroupDto, string newGroupName, int courseId)
    {
        if (courseId == default)
        {
            _logger.LogError($"Error in GroupService - CreateGroup while fetching course with Id [{courseId}]");
            throw new Exception($"There is no course with Id [{courseId}] in DB set.");
        }
        
        if (string.IsNullOrWhiteSpace(newGroupName))
        {
            _logger.LogError("Error in GroupService - CreateGroup new created group name is null or empty.");
            throw new Exception("New created group name is null or empty.");
        }
        
        var existingGroup = await _dbContext.Groups.FirstOrDefaultAsync(g => g.Name == newGroupName);

        if (existingGroup != null)
        {
            _logger.LogError($"Error in GroupService - CreateGroup group with name [{newGroupName}] already exists.");
            throw new Exception($"Group with name '{newGroupName}' already exists.");
        }
        
        var groupEntity = _mapper.Map<Group>(newGroupDto);
        
        groupEntity.Name = newGroupName;
        groupEntity.CourseId = courseId;

        await Add(groupEntity);
        
        _logger.LogInformation($"New group with name [{newGroupName}] is successfully created within course with Id:[{courseId}]");
        return _mapper.Map<GroupDto>(groupEntity);
    }

    public async Task DeleteGroup(int groupId)
    {
        var group = await GetById(groupId);
        
        if (group == null)
        {
            _logger.LogError($"Error in GroupService - DeleteGroup while fetching group with Id [{groupId}]");
            throw new Exception($"There is no group with Id [{groupId}] in DB set.");
        }
        
        if (_dbContext.Students.Any(s => s.GroupId == groupId))
        {
            _logger.LogError($"Error in GroupService - DeleteGroup: Group with Id [{groupId}] cannot be deleted as it has associated students.");
            throw new Exception($"Group with Id [{groupId}] cannot be deleted as it has associated students.");
        }
        
        _logger.LogInformation($"Group with Id [{groupId}] was successfully deleted");
        await Delete(groupId);
    }
}