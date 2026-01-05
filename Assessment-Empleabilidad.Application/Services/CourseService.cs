using Assessment_Empleabilidad.Application.DTOs;
using Assessment_Empleabilidad.Application.Interfaces;
using Assessment_Empleabilidad.Domain.Entities;
using Assessment_Empleabilidad.Domain.Enums;
using Assessment_Empleabilidad.Domain.Interfaces;

namespace Assessment_Empleabilidad.Application.Services;

public class CourseService : ICourseService
{
    private readonly ICourseRepository _courseRepository;

    public CourseService(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public async Task<CourseDtos.CourseResponseDto?> GetCourseByIdAsync(Guid id)
    {
        var course = await _courseRepository.GetCourseById(id);
        return course == null ? null : MapToDto(course);
    }

    public async Task<CourseDtos.CourseSummaryDto?> GetCourseSummaryAsync(Guid id)
    {
        var course = await _courseRepository.GetCourseById(id);
        if (course == null) return null;

        return new CourseDtos.CourseSummaryDto
        {
            Id = course.Id,
            Title = course.Title,
            Status = course.Status,
            TotalLessons = course.Lessons.Count(l => !l.IsDeleted), // Contar solo activas
            LastModified = course.UpdatedAt ?? course.CreatedAt
        };
    }

    public async Task<(IEnumerable<CourseDtos.CourseResponseDto> Items, int TotalCount)> SearchCoursesAsync(
        string? searchTerm, CourseStatus? status, int page, int pageSize)
    {
        var result = await _courseRepository.SearchCourses(searchTerm, status, page, pageSize);
        
        var dtos = result.Items.Select(MapToDto);
        return (dtos, result.TotalCount);
    }

    public async Task<CourseDtos.CourseResponseDto> AddCourseAsync(CourseDtos.CourseCreateDto dto)
    {
        var course = new Course
        {
            Title = dto.Title,
            Status = CourseStatus.Draft // Por defecto Draft
        };

        var created = await _courseRepository.AddCourse(course);
        return MapToDto(created);
    }

    public async Task<CourseDtos.CourseResponseDto?> UpdateCourseAsync(CourseDtos.CourseUpdateDto dto)
    {
        var course = new Course
        {
            Title = dto.Title,
            Status = dto.Status
        };

        var updated = await _courseRepository.UpdateCourse(dto.Id, course);
        return updated == null ? null : MapToDto(updated);
    }

    public async Task<bool> DeleteCourseAsync(Guid id)
    {
        var deleted = await _courseRepository.DeleteCourse(id);
        return deleted != null;
    }

    public async Task<bool> PublishCourseAsync(Guid id)
    {
        var course = await _courseRepository.GetCourseById(id);
        if (course == null) return false;

        // REGLA DE NEGOCIO: Debe tener lecciones activas
        if (!course.Lessons.Any(l => !l.IsDeleted))
        {
            throw new InvalidOperationException("No se puede publicar un curso sin lecciones activas.");
        }

        course.Status = CourseStatus.Published;
        await _courseRepository.UpdateCourse(id, course);
        return true;
    }

    public async Task<bool> UnpublishCourseAsync(Guid id)
    {
        var course = await _courseRepository.GetCourseById(id);
        if (course == null) return false;

        course.Status = CourseStatus.Draft;
        await _courseRepository.UpdateCourse(id, course);
        return true;
    }

    // Mapper manual simple
    private static CourseDtos.CourseResponseDto MapToDto(Course course)
    {
        return new CourseDtos.CourseResponseDto
        {
            Id = course.Id,
            Title = course.Title,
            Status = course.Status,
            CreatedAt = course.CreatedAt,
            UpdatedAt = course.UpdatedAt
        };
    }
}