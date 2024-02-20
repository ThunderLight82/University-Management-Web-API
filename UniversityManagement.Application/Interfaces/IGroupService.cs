using UniversityManagement.Domain.Entities;
using UniversityManagement.DTO.EntitiesDto;

namespace UniversityManagement.Application.Interfaces;

public interface IGroupService : IBaseService<Group>
{
    Task<GroupDto> GetGroupById(int groupId);
    Task<IEnumerable<GroupDto>> GetGroups();
    Task<IEnumerable<StudentDto>> GetStudentsByGroupId(int groupId);
    Task<GroupDto> UpdateGroup(GroupDto groupDto);
    Task<GroupDto> CreateGroup(GroupDto newGroupDto);
    Task DeleteGroup(GroupDto groupDto);
}