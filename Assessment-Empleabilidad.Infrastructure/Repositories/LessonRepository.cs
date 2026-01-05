using Assessment_Empleabilidad.Domain.Entities;
using Assessment_Empleabilidad.Domain.Interfaces;
using Assessment_Empleabilidad.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Assessment_Empleabilidad.Infrastructure.Repositories;

public class LessonRepository : ILessonRepository
{
    private readonly AppDbContext _context;
    
    public LessonRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<Lesson?> GetLessonById(Guid lessonId)
    {
        return await _context.Lessons
            .Include(l => l.Course)
            .FirstOrDefaultAsync(l => l.Id == lessonId && !l.IsDeleted);
    }

    public async Task<Lesson?> GetLessonByName(string lessonName)
    {
        return await _context.Lessons
            .Include(l => l.Course)
            .FirstOrDefaultAsync(l => l.Title.ToLower() == lessonName.ToLower() && !l.IsDeleted);
    }

    public async Task<IEnumerable<Lesson>> GetLessonsByCourseId(Guid courseId)
    {
        return await _context.Lessons
            .Include(l => l.Course)
            .Where(l => l.CourseId == courseId && !l.IsDeleted)
            .OrderBy(l => l.Order)
            .ThenBy(l => l.Title)
            .ToListAsync();
    }

    public async Task<IEnumerable<Lesson>> GetAllLessons()
    {
        return await _context.Lessons
            .Include(l => l.Course)
            .Where(l => !l.IsDeleted)
            .OrderBy(l => l.Course.Title)
            .ThenBy(l => l.Order)
            .ThenBy(l => l.Title)
            .ToListAsync();
    }

    public async Task<Lesson> AddLesson(Lesson lesson)
    {
        // Verificar que el curso existe y no está eliminado
        var courseExists = await _context.Courses
            .AnyAsync(c => c.Id == lesson.CourseId && !c.IsDeleted);
            
        if (!courseExists)
        {
            throw new ArgumentException("El curso especificado no existe o ha sido eliminado.");
        }
        
        lesson.Id = Guid.NewGuid();
        lesson.CreatedAt = DateTime.UtcNow;
        lesson.UpdatedAt = null;
        lesson.IsDeleted = false;

        await _context.Lessons.AddAsync(lesson);
        await _context.SaveChangesAsync();
        
        // Cargar la relación Course
        await _context.Entry(lesson).Reference(l => l.Course).LoadAsync();
        
        return lesson;
    }

    public async Task<Lesson?> UpdateLesson(Lesson lesson)
    {
        var existing = await _context.Lessons
            .Include(l => l.Course)
            .FirstOrDefaultAsync(l => l.Id == lesson.Id && !l.IsDeleted);

        if (existing != null)
        {
            // Si se cambia el curso, verificar que existe
            if (existing.CourseId != lesson.CourseId)
            {
                var courseExists = await _context.Courses
                    .AnyAsync(c => c.Id == lesson.CourseId && !c.IsDeleted);
                    
                if (!courseExists)
                {
                    throw new ArgumentException("El nuevo curso especificado no existe o ha sido eliminado.");
                }
            }
            
            existing.Title = lesson.Title;
            existing.Order = lesson.Order;
            existing.CourseId = lesson.CourseId;
            existing.UpdatedAt = DateTime.UtcNow;

            _context.Lessons.Update(existing);
            await _context.SaveChangesAsync();
            return existing;
        }
        
        return null;
    }

    public async Task<Lesson?> DeleteLesson(Guid lessonId)
    {
        var lesson = await _context.Lessons
            .Include(l => l.Course)
            .FirstOrDefaultAsync(l => l.Id == lessonId && !l.IsDeleted);

        if (lesson != null)
        {
            // Soft delete
            lesson.IsDeleted = true;
            lesson.UpdatedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            return lesson;
        }
        
        return null;
    }
}