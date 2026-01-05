using Assessment_Empleabilidad.Domain.Entities;

namespace Assessment_Empleabilidad.Domain.Interfaces;

public interface ILessonRepository
{
    public Task<Lesson> GetLessonById(Guid lessonId);
    public Task<Lesson> GetLessonByName(string lessonName);
    public Task<IEnumerable<Lesson>> GetLessonsByCourseId(Guid courseId);
    public Task<IEnumerable<Lesson>> GetAllLessons();
    
    public Task<Lesson> AddLesson(Lesson lesson);
    public Task<Lesson> UpdateLesson(Lesson lesson);
    public Task<Lesson> DeleteLesson(Guid lessonId);
}