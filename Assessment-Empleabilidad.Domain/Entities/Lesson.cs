using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Assessment_Empleabilidad.Domain.Entities;

public class Lesson
{
    [Key] public Guid Id { get; set; }
    [Column(TypeName = "varchar(100)")] public string Title { get; set; }
    public int Order { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    //FK
    public Guid CourseId { get; set; }
    public Course Course { get; set; }
    
}