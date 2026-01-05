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
        var courseId = Guid.NewGuid();
        var existingLessons = new List<Lesson>
        {
            new Lesson { Order = 1 },
            new Lesson { Order = 2 },
            new Lesson { Order = 5 }
        };

        _mockRepo.Setup(repo => repo.GetLessonsByCourseId(courseId))
            .ReturnsAsync(existingLessons);
        
        _mockRepo.Setup(repo => repo.AddLesson(It.IsAny<Lesson>()))
            .ReturnsAsync((Lesson l) => l);

        var dto = new LessonDtos.LessonCreateDto
        {
            CourseId = courseId,
            Title = "Nueva Lección"
        };
        
        var result = await _service.AddLessonAsync(dto);
        
        Assert.Equal(6, result.Order); 
    }

    [Fact]
    public async Task CreateLesson_FirstLesson_ShouldBeOrderOne()
    {
        var courseId = Guid.NewGuid();
        
        _mockRepo.Setup(repo => repo.GetLessonsByCourseId(courseId))
            .ReturnsAsync(new List<Lesson>());

        _mockRepo.Setup(repo => repo.AddLesson(It.IsAny<Lesson>()))
            .ReturnsAsync((Lesson l) => l);

        var dto = new LessonDtos.LessonCreateDto { CourseId = courseId, Title = "Primera" };
        
        var result = await _service.AddLessonAsync(dto);
        
        Assert.Equal(1, result.Order);
    }
    
    [Fact]
    public async Task ReorderLesson_ShouldUpdateOrdersCorrectly()
    {
        var courseId = Guid.NewGuid();
        var lesson1 = new Lesson { Id = Guid.NewGuid(), Order = 1, CourseId = courseId };
        var lesson2 = new Lesson { Id = Guid.NewGuid(), Order = 2, CourseId = courseId };
        var lesson3 = new Lesson { Id = Guid.NewGuid(), Order = 3, CourseId = courseId };
        
        var lessons = new List<Lesson> { lesson1, lesson2, lesson3 };

        _mockRepo.Setup(repo => repo.GetLessonsByCourseId(courseId))
            .ReturnsAsync(lessons);
        
        await _service.ReorderLessonAsync(courseId, lesson3.Id, 1);
        
        Assert.Equal(1, lesson3.Order);
        _mockRepo.Verify(repo => repo.UpdateLesson(It.Is<Lesson>(l => l.Id == lesson3.Id && l.Order == 1)), Times.Once);
    }
}