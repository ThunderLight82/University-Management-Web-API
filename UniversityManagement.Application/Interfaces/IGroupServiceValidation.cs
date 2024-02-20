using UniversityManagement.Domain.Entities;
using UniversityManagement.DTO.EntitiesDto;

namespace UniversityManagement.Application.Interfaces;

public interface IGroupServiceValidation
{
    void ValidateGetGroupsById(int groupId);
    void ValidateGetGroups(IEnumerable<Group> groups);
    public void ValidateGetStudentsByGroupId(int groupId);
    void ValidateUpdateGroup(GroupDto groupDto);
    void ValidateCreateGroup(GroupDto newGroupDto);
    void ValidateDeleteGroup(GroupDto groupDto);
}