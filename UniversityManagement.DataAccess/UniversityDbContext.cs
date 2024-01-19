using Microsoft.EntityFrameworkCore;
using UniversityManagement.Domain.Entities;

namespace UniversityManagement.DataAccess;

public class UniversityDbContext : DbContext
{
    public DbSet<Course> Courses { get; set; } = null!;
    public DbSet<Group> Groups { get; set; } = null!;
    public DbSet<Student> Students { get; set; } = null!;
    
    public UniversityDbContext(DbContextOptions<UniversityDbContext> options) : base(options) { }
    
    // Some design parameters for entities and initially created course list
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Course>().HasData(
            new() { Id = 1, Name = "System Engineer" },
            new() { Id = 2, Name = "Software Engineer" },
            new() { Id = 3, Name = "Data Science" },
            new() { Id = 4, Name = "Data Analysis" },
            new() { Id = 5, Name = "Cyber Security" }
        );
        
        modelBuilder.Entity<Group>()
            .HasIndex(g => g.Name)
            .IsUnique();
    }
}