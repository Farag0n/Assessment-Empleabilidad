namespace Assessment_Empleabilidad.Application.DTOs;

public class LessonDtos
{
    public class LessonCreateDto
    {
        // IMPORTANTE: Necesitamos saber a qué curso pertenece la lección
        public Guid CourseId { get; set; } 
        
        public string Title { get; set; } = string.Empty;
        
        // Eliminamos 'Order' de aquí porque lo calculamos en el Service automáticamente
    }

    public class LessonUpdateDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        // Eliminamos IsDeleted de aquí, eso se maneja con DeleteLessonAsync
    }
    
    public class LessonResponseDto
    {
        public Guid Id { get; set; }
        public Guid CourseId { get; set; }
        public string Title { get; set; } = string.Empty;
        public int Order { get; set; }
        public bool IsDeleted { get; set; }
    }
    
    // DTO específico para la funcionalidad de reordenar
    public class LessonReorderDto
    {
        public Guid LessonId { get; set; }
        public int NewOrder { get; set; }
    }
}