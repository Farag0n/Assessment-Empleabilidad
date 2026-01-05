using Assessment_Empleabilidad.Application.Services;
using Assessment_Empleabilidad.Domain.Entities;
using Assessment_Empleabilidad.Domain.Enums;
using Assessment_Empleabilidad.Domain.Interfaces;
using Moq;
using Xunit;

namespace Assessment_Empleabilidad.Tests;

public class CourseServiceTests
{
    // Simulamos el repositorio, no usamos la BD real
    private readonly Mock<ICourseRepository> _mockRepo;
    private readonly CourseService _service;

    public CourseServiceTests()
    {
        _mockRepo = new Mock<ICourseRepository>();
        _service = new CourseService(_mockRepo.Object);
    }

    [Fact]
    public async Task PublishCourse_WithLessons_ShouldSucceed()
    {
        // Arrange (Preparar)
        var courseId = Guid.NewGuid();
        var course = new Course
        {
            Id = courseId,
            Status = CourseStatus.Draft,
            // Simulamos que TIENE una lección activa
            Lessons = new List<Lesson> { new Lesson { IsDeleted = false } }
        };

        _mockRepo.Setup(repo => repo.GetCourseById(courseId))
            .ReturnsAsync(course);

        // Act (Ejecutar)
        var result = await _service.PublishCourseAsync(courseId);

        // Assert (Verificar)
        Assert.True(result); // Debe retornar true
        Assert.Equal(CourseStatus.Published, course.Status); // El estado debe haber cambiado
        
        // Verificamos que se llamó al Update en el repositorio
        _mockRepo.Verify(repo => repo.UpdateCourse(courseId, It.IsAny<Course>()), Times.Once);
    }

    [Fact]
    public async Task PublishCourse_WithoutLessons_ShouldFail()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var course = new Course
        {
            Id = courseId,
            Status = CourseStatus.Draft,
            Lessons = new List<Lesson>() // Lista VACÍA
        };

        _mockRepo.Setup(repo => repo.GetCourseById(courseId))
            .ReturnsAsync(course);

        // Act & Assert
        // Esperamos que lance una excepción InvalidOperationException
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _service.PublishCourseAsync(courseId));
    }

    [Fact]
    public async Task DeleteCourse_ShouldBeSoftDelete()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        
        // Simulamos que el repositorio devuelve el curso "borrado" (soft delete)
        // Nota: En tu implementación, el repositorio hace el soft delete internamente.
        // Aquí verificamos que el servicio llame al método correcto del repositorio.
        _mockRepo.Setup(repo => repo.DeleteCourse(courseId))
            .ReturnsAsync(new Course { Id = courseId, IsDeleted = true });

        // Act
        var result = await _service.DeleteCourseAsync(courseId);

        // Assert
        Assert.True(result);
        _mockRepo.Verify(repo => repo.DeleteCourse(courseId), Times.Once);
    }
}