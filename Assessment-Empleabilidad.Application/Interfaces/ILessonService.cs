using Assessment_Empleabilidad.Application.DTOs;

namespace Assessment_Empleabilidad.Application.Interfaces;

public interface ILessonService
{
    Task<LessonDtos.LessonResponseDto?> GetLessonByIdAsync(Guid id);
    Task<IEnumerable<LessonDtos.LessonResponseDto>> GetLessonsByCourseIdAsync(Guid courseId);
    
    Task<LessonDtos.LessonResponseDto> AddLessonAsync(LessonDtos.LessonCreateDto lessonCreateDto);
    Task<LessonDtos.LessonResponseDto?> UpdateLessonAsync(LessonDtos.LessonUpdateDto lessonUpdateDto);
    Task<bool> DeleteLessonAsync(Guid id);
    
    Task<bool> ReorderLessonAsync(Guid courseId, Guid lessonId, int newOrder);
}