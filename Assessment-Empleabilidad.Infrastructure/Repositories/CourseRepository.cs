using Assessment_Empleabilidad.Domain.Entities;
using Assessment_Empleabilidad.Domain.Enums;
using Assessment_Empleabilidad.Domain.Interfaces;
using Assessment_Empleabilidad.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Assessment_Empleabilidad.Infrastructure.Repositories;

public class CourseRepository : ICourseRepository
{
    private readonly AppDbContext _context;
    
    public CourseRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Course?> GetCourseById(Guid courseId)
    {
        return await _context.Courses
            .Include(c => c.Lessons)
            .FirstOrDefaultAsync(c => c.Id == courseId && !c.IsDeleted);
    }

    public async Task<Course?> GetCourseByName(string courseName)
    {
        return await _context.Courses
            .Include(c => c.Lessons)
            .FirstOrDefaultAsync(c => c.Title.ToLower() == courseName.ToLower() && !c.IsDeleted);
    }

    public async Task<IEnumerable<Course>> GetAllCourses()
    {
        return await _context.Courses
            .Include(c => c.Lessons)
            .Where(c => !c.IsDeleted)
            .OrderBy(c => c.Title)
            .ToListAsync();
    }

    public async Task<(IEnumerable<Course> Items, int TotalCount)> SearchCourses(
        string? searchTerm, 
        CourseStatus? status, 
        int page, 
        int pageSize)
    {
        var query = _context.Courses
            .Include(c => c.Lessons)
            .Where(c => !c.IsDeleted);
        
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(c => c.Title.Contains(searchTerm));
        }

        if (status.HasValue)
        {
            query = query.Where(c => c.Status == status.Value);
        }
        
        var totalCount = await query.CountAsync();
        
        var items = await query
            .OrderBy(c => c.Title)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<Course> AddCourse(Course course)
    {
        course.Id = Guid.NewGuid();
        course.CreatedAt = DateTime.UtcNow;
        course.UpdatedAt = null;
        course.IsDeleted = false;

        await _context.Courses.AddAsync(course);
        await _context.SaveChangesAsync();
        return course;
    }

    public async Task<Course?> UpdateCourse(Guid courseId, Course course)
    {
        var existing = await _context.Courses
            .FirstOrDefaultAsync(c => c.Id == courseId && !c.IsDeleted);

        if (existing != null)
        {
            existing.Title = course.Title;
            existing.Status = course.Status;
            existing.UpdatedAt = DateTime.UtcNow;

            _context.Courses.Update(existing);
            await _context.SaveChangesAsync();
            return existing;
        }
        
        return null;
    }

    public async Task<Course?> DeleteCourse(Guid courseId)
    {
        var course = await _context.Courses
            .FirstOrDefaultAsync(c => c.Id == courseId && !c.IsDeleted);

        if (course != null)
        {
            // Soft delete
            course.IsDeleted = true;
            course.UpdatedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            return course;
        }
        
        return null;
    }
}