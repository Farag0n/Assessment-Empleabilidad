using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Assessment_Empleabilidad.Domain.Enums;

namespace Assessment_Empleabilidad.Domain.Entities;

public class Course
{
    [Key] public Guid Id { get; set; }
    [Column(TypeName = "varchar(100)")] public string Title { get; set; }
    public CourseStatus Status { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();

}