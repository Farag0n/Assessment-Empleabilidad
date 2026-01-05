using Assessment_Empleabilidad.Application.DTOs;

namespace Assessment_Empleabilidad.Application.Interfaces;

public interface ICourseService
{
    Task<CourseDtos.CourseResponseDto?> GetCourseByIdAsync(Guid id);
    Task<CourseDtos.CourseSummaryDto?> GetCourseSummaryAsync(Guid id);
    
    Task<(IEnumerable<CourseDtos.CourseResponseDto> Items, int TotalCount)> SearchCoursesAsync(
        string? searchTerm, 
        Domain.Enums.CourseStatus? status, 
        int page, 
        int pageSize);

    Task<CourseDtos.CourseResponseDto> AddCourseAsync(CourseDtos.CourseCreateDto courseCreateDto);
    Task<CourseDtos.CourseResponseDto?> UpdateCourseAsync(CourseDtos.CourseUpdateDto courseUpdateDto);
    Task<bool> DeleteCourseAsync(Guid id);
    
    Task<bool> PublishCourseAsync(Guid id);
    Task<bool> UnpublishCourseAsync(Guid id);
}