namespace UniversityManagement.Domain.Entities;

public class Group : BaseEntity
{
    public string Name { get; set; }
    
    public List<Student> Students { get; set; }
    
    public int CourseId { get; set; }
    
    public Course Course { get; set; }
}