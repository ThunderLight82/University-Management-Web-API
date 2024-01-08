using UniversityManagement.Application.EntitiesDto;
using UniversityManagement.Domain.Entities;

namespace UniversityManagement.Application.Services.Interfaces;

public interface IGroupService : IBaseService<Group>
{
    Task<GroupDto> GetGroupById(int groupId);
    Task<IEnumerable<GroupDto>> GetGroupsAll();
    Task<IEnumerable<GroupDto>> GetGroupsAllByCourseId(int courseId);
    Task<GroupDto> ChangeGroupName(string newChangedGroupName, int groupId);
    Task<GroupDto> CreateGroup(GroupDto newGroupDto, string newGroupName, int courseId);
    Task DeleteGroup(int groupId);
}