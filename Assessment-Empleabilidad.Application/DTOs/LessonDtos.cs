namespace Assessment_Empleabilidad.Application.DTOs;

public class LessonDtos
{
    public class LessonCreateDto
    {
        public Guid CourseId { get; set; } 
        
        public string Title { get; set; } = string.Empty;
    }

    public class LessonUpdateDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
    }
    
    public class LessonResponseDto
    {
        public Guid Id { get; set; }
        public Guid CourseId { get; set; }
        public string Title { get; set; } = string.Empty;
        public int Order { get; set; }
        public bool IsDeleted { get; set; }
    }
    
    public class LessonReorderDto
    {
        public Guid LessonId { get; set; }
        public int NewOrder { get; set; }
    }
}