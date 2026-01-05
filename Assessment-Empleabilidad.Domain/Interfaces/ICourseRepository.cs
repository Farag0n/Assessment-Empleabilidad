using Assessment_Empleabilidad.Domain.Entities;
using Assessment_Empleabilidad.Domain.Enums;

namespace Assessment_Empleabilidad.Domain.Interfaces;

public interface ICourseRepository
{
    public Task<Course?> GetCourseById(Guid courseId);
    public Task<Course?> GetCourseByName(string courseName);
    public Task<IEnumerable<Course>> GetAllCourses();
    
    public Task<(IEnumerable<Course> Items, int TotalCount)> SearchCourses(
        string? searchTerm, 
        CourseStatus? status, 
        int page,
        int pageSize
    );
    
    public Task<Course> AddCourse(Course course);
    public Task<Course?> UpdateCourse(Guid courseId, Course course);
    public Task<Course?> DeleteCourse(Guid courseId);
}