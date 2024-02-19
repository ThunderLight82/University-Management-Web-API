using UniversityManagement.Domain.Entities;

namespace UniversityManagement.DTO.EntitiesDto;

public class CourseDto : BaseEntityDto
{
    public string Name { get; set; }
    
    public List<Group> Groups { get; set; }
}