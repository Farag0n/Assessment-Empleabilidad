using Assessment_Empleabilidad.Application.DTOs;
using Assessment_Empleabilidad.Application.Services;
using Assessment_Empleabilidad.Domain.Entities;
using Assessment_Empleabilidad.Domain.Interfaces;
using Moq;
using Xunit;

namespace Assessment_Empleabilidad.Tests;

public class LessonServiceTests
{
    private readonly Mock<ILessonRepository> _mockRepo;
    private readonly LessonService _service;

    public LessonServiceTests()
    {
        _mockRepo = new Mock<ILessonRepository>();
        _service = new LessonService(_mockRepo.Object);
    }

    [Fact]
    public async Task CreateLesson_ShouldAssignNextOrder_EnsuringUniqueness()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var existingLessons = new List<Lesson>
        {
            new Lesson { Order = 1 },
            new Lesson { Order = 2 },
            new Lesson { Order = 5 } // Hay huecos, el máximo es 5
        };

        _mockRepo.Setup(repo => repo.GetLessonsByCourseId(courseId))
            .ReturnsAsync(existingLessons);

        // Simulamos que el repositorio devuelve la lección creada para que el servicio no falle
        _mockRepo.Setup(repo => repo.AddLesson(It.IsAny<Lesson>()))
            .ReturnsAsync((Lesson l) => l);

        var dto = new LessonDtos.LessonCreateDto
        {
            CourseId = courseId,
            Title = "Nueva Lección"
        };

        // Act
        var result = await _service.AddLessonAsync(dto);

        // Assert
        // La lógica debe ser Max(5) + 1 = 6. 
        // Esto valida "CreateLesson_WithUniqueOrder_ShouldSucceed"
        Assert.Equal(6, result.Order); 
    }

    [Fact]
    public async Task CreateLesson_FirstLesson_ShouldBeOrderOne()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        // No hay lecciones existentes
        _mockRepo.Setup(repo => repo.GetLessonsByCourseId(courseId))
            .ReturnsAsync(new List<Lesson>());

        _mockRepo.Setup(repo => repo.AddLesson(It.IsAny<Lesson>()))
            .ReturnsAsync((Lesson l) => l);

        var dto = new LessonDtos.LessonCreateDto { CourseId = courseId, Title = "Primera" };

        // Act
        var result = await _service.AddLessonAsync(dto);

        // Assert
        Assert.Equal(1, result.Order);
    }
    
    // Test extra para cubrir la lógica de reordenamiento
    [Fact]
    public async Task ReorderLesson_ShouldUpdateOrdersCorrectly()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var lesson1 = new Lesson { Id = Guid.NewGuid(), Order = 1, CourseId = courseId };
        var lesson2 = new Lesson { Id = Guid.NewGuid(), Order = 2, CourseId = courseId };
        var lesson3 = new Lesson { Id = Guid.NewGuid(), Order = 3, CourseId = courseId };
        
        var lessons = new List<Lesson> { lesson1, lesson2, lesson3 };

        _mockRepo.Setup(repo => repo.GetLessonsByCourseId(courseId))
            .ReturnsAsync(lessons);

        // Act: Movemos la Lección 3 a la posición 1
        await _service.ReorderLessonAsync(courseId, lesson3.Id, 1);

        // Assert
        // La lección 3 ahora debe ser 1
        Assert.Equal(1, lesson3.Order);
        // La lección 1 debió bajar a 2 (porque la empujaron)
        // Nota: En un test unitario con Moq, los objetos en memoria se actualizan si son referencia,
        // pero verificamos las llamadas al Update.
        _mockRepo.Verify(repo => repo.UpdateLesson(It.Is<Lesson>(l => l.Id == lesson3.Id && l.Order == 1)), Times.Once);
    }
}