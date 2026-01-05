using Assessment_Empleabilidad.Application.DTOs;
using Assessment_Empleabilidad.Application.Interfaces;
using Assessment_Empleabilidad.Domain.Entities;
using Assessment_Empleabilidad.Domain.Interfaces;

namespace Assessment_Empleabilidad.Application.Services;

public class LessonService : ILessonService
{
    private readonly ILessonRepository _lessonRepository;

    public LessonService(ILessonRepository lessonRepository)
    {
        _lessonRepository = lessonRepository;
    }

    public async Task<LessonDtos.LessonResponseDto?> GetLessonByIdAsync(Guid id)
    {
        var lesson = await _lessonRepository.GetLessonById(id);
        return lesson == null ? null : MapToDto(lesson);
    }

    public async Task<IEnumerable<LessonDtos.LessonResponseDto>> GetLessonsByCourseIdAsync(Guid courseId)
    {
        var lessons = await _lessonRepository.GetLessonsByCourseId(courseId);
        return lessons.Select(MapToDto);
    }

    public async Task<LessonDtos.LessonResponseDto> AddLessonAsync(LessonDtos.LessonCreateDto dto)
    {
        // REGLA: Calcular el orden automáticamente (Max + 1)
        var existingLessons = await _lessonRepository.GetLessonsByCourseId(dto.CourseId);
        int newOrder = 1;
        
        if (existingLessons.Any())
        {
            newOrder = existingLessons.Max(l => l.Order) + 1;
        }

        var lesson = new Lesson
        {
            Title = dto.Title,
            CourseId = dto.CourseId,
            Order = newOrder,
            IsDeleted = false
        };

        var created = await _lessonRepository.AddLesson(lesson);
        return MapToDto(created);
    }

    public async Task<LessonDtos.LessonResponseDto?> UpdateLessonAsync(LessonDtos.LessonUpdateDto dto)
    {
        var existing = await _lessonRepository.GetLessonById(dto.Id);
        if (existing == null) return null;

        existing.Title = dto.Title;
        // Nota: No actualizamos el Order aquí, usamos ReorderLessonAsync

        var updated = await _lessonRepository.UpdateLesson(existing);
        return MapToDto(updated);
    }

    public async Task<bool> DeleteLessonAsync(Guid id)
    {
        var deleted = await _lessonRepository.DeleteLesson(id);
        return deleted != null;
    }

    // LÓGICA COMPLEJA: Reordenamiento sin duplicados
    public async Task<bool> ReorderLessonAsync(Guid courseId, Guid lessonId, int newOrder)
    {
        var lessons = (await _lessonRepository.GetLessonsByCourseId(courseId)).ToList();
        var lessonToMove = lessons.FirstOrDefault(l => l.Id == lessonId);

        if (lessonToMove == null) return false;
        if (newOrder < 1 || newOrder > lessons.Count) throw new ArgumentException("Orden inválido.");
        if (lessonToMove.Order == newOrder) return true; // No hay cambios

        var oldOrder = lessonToMove.Order;

        // Estrategia: Desplazar los elementos afectados
        if (newOrder > oldOrder) // Mover hacia abajo (ej: de 2 a 4)
        {
            // Restar 1 a los que están entre oldOrder + 1 y newOrder
            var lessonsToShift = lessons.Where(l => l.Order > oldOrder && l.Order <= newOrder);
            foreach (var l in lessonsToShift)
            {
                l.Order--;
                await _lessonRepository.UpdateLesson(l);
            }
        }
        else // Mover hacia arriba (ej: de 4 a 2)
        {
            // Sumar 1 a los que están entre newOrder y oldOrder - 1
            var lessonsToShift = lessons.Where(l => l.Order >= newOrder && l.Order < oldOrder);
            foreach (var l in lessonsToShift)
            {
                l.Order++;
                await _lessonRepository.UpdateLesson(l);
            }
        }

        // Asignar nuevo orden a la lección objetivo
        lessonToMove.Order = newOrder;
        await _lessonRepository.UpdateLesson(lessonToMove);

        return true;
    }

    private static LessonDtos.LessonResponseDto MapToDto(Lesson lesson)
    {
        return new LessonDtos.LessonResponseDto
        {
            Id = lesson.Id,
            CourseId = lesson.CourseId,
            Title = lesson.Title,
            Order = lesson.Order,
            IsDeleted = lesson.IsDeleted
        };
    }
}