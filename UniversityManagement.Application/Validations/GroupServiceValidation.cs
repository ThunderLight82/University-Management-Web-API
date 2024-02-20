using Microsoft.Extensions.Logging;
using UniversityManagement.Application.Interfaces;
using UniversityManagement.Domain.Entities;
using UniversityManagement.DTO.EntitiesDto;

namespace UniversityManagement.Application.Validations;

public class GroupServiceValidation : IGroupServiceValidation
{
    private readonly ILogger<GroupServiceValidation> _logger;

    public GroupServiceValidation(ILogger<GroupServiceValidation> logger)
    {
        _logger = logger;
    }
    
    public void ValidateGetGroupsById(int groupId)
    {
        if (groupId == default)
        {
            _logger.LogError($"Error in GroupService - GetGroupById while fetching group by Id [{groupId}]");
            throw new Exception($"There is no group with Id [{groupId}] in DB set.");
        }
    }
    
    public void ValidateGetGroups(IEnumerable<Group> groups)
    {
        if (groups == null)
        {
            _logger.LogError("Error in GroupService - GetGroups while fetching all group.");
            throw new Exception("Failed to retrieve group list from DB set.");
        }
    }
    
    public void ValidateGetStudentsByGroupId(int groupId)
    {
        if (groupId == default)
        {
            _logger.LogError($"Error in GroupService - GetStudentsListByGroupId while fetching students by group Id [{groupId}]");
            throw new Exception($"There is no group with Id [{groupId}] in DB set.");
        }
    }
    
    public void ValidateUpdateGroup(GroupDto groupDto)
    {
        if (groupDto.Id == default)
        {
            _logger.LogError($"Error in GroupService - UpdateGroup while fetching group with Id [{groupDto.Id}]");
            throw new Exception($"There is no group with Id [{groupDto.Id}] in DB set.");
        }

        if (string.IsNullOrWhiteSpace(groupDto.Name))
        {
            _logger.LogError($"Error in GroupService - UpdateGroup new group name is null or empty for group with Id [{groupDto.Id}]");
            throw new Exception($"New group name is null or empty for group with Id [{groupDto.Id}]");
        }
    }
    
    public void ValidateCreateGroup(GroupDto newGroupDto)
    {
        if (newGroupDto.CourseId == default)
        {
            _logger.LogError($"Error in GroupService - CreateGroup while fetching course with Id [{newGroupDto.CourseId}]");
            throw new Exception($"There is no course with Id [{newGroupDto.CourseId}] in DB set.");
        }
        
        if (string.IsNullOrWhiteSpace(newGroupDto.Name))
        {
            _logger.LogError("Error in GroupService - CreateGroup new created group name is null or empty.");
            throw new Exception("New created group name is null or empty.");
        }
    }

    public void ValidateDeleteGroup(GroupDto groupDto)
    {
        if (groupDto.Id == default)
        {
            _logger.LogError($"Error in GroupService - DeleteGroup while fetching group with Id [{groupDto.Id}]");
            throw new Exception($"There is no group with Id [{groupDto.Id}] in DB set.");
        }
    }
}