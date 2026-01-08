using Assessment_Empleabilidad.Domain.Entities;
using Assessment_Empleabilidad.Domain.Enums;

namespace Assessment_Empleabilidad.Domain.Interfaces;

public interface ICourseRepository
{
    public Task<Course?> GetCourseById(Guid courseId);
    public Task<Course?> GetCourseByName(string courseName);
    public Task<IEnumerable<Course>> GetAllCourses();
    
    //esto es retorna un ienumerable llamado items  y el total de courses 
    public Task<(IEnumerable<Course> Items, int TotalCount)> SearchCourses(
        string? searchTerm, 
        CourseStatus? status, 
        int page, //para escoger desde que bloque de resultados se desea
        int pageSize //para escoger cuantos resultados retorna
    );
    
    public Task<Course> AddCourse(Course course);
    public Task<Course?> UpdateCourse(Guid courseId, Course course);
    public Task<Course?> DeleteCourse(Guid courseId);
}