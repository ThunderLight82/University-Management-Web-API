using UniversityManagement.Domain.Entities;

namespace UniversityManagement.Application.EntitiesDto;

public class CourseDto : BaseEntityDto
{
    public string Name { get; set; }
    
    public List<Group> Groups { get; set; }
}