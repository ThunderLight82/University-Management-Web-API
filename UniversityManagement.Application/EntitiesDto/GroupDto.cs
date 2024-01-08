using UniversityManagement.Domain.Entities;

namespace UniversityManagement.Application.EntitiesDto;

public class GroupDto : BaseEntityDto
{
    public string Name { get; set; }
    
    public List<Student> Students { get; set; }
    
    public int CourseId { get; set; }

    public Course Course { get; set; }
}