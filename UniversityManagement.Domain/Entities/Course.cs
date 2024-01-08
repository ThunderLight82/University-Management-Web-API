namespace UniversityManagement.Domain.Entities;

public class Course : BaseEntity
{
    public string Name { get; set; }
    
    public List<Group> Groups { get; set; }
}