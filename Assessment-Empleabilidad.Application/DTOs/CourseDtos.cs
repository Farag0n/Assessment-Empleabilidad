using Assessment_Empleabilidad.Domain.Enums;

namespace Assessment_Empleabilidad.Application.DTOs;

public class CourseDtos
{
    public class CourseCreateDto
    {
        public string Title { get; set; } = string.Empty;
        public CourseStatus Status { get; set; } = CourseStatus.Draft;
    }
    
    public class CourseUpdateDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public CourseStatus Status { get; set; }
    }
    
    public class CourseResponseDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public CourseStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
    
    public class CourseSummaryDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public CourseStatus Status { get; set; }
        public int TotalLessons { get; set; }
        public DateTime? LastModified { get; set; }
    }
}