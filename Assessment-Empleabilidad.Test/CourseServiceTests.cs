using Assessment_Empleabilidad.Application.Services;
using Assessment_Empleabilidad.Domain.Entities;
using Assessment_Empleabilidad.Domain.Enums;
using Assessment_Empleabilidad.Domain.Interfaces;
using Moq;
using Xunit;

namespace Assessment_Empleabilidad.Tests;

public class CourseServiceTests
{
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
        var courseId = Guid.NewGuid();
        var course = new Course
        {
            Id = courseId,
            Status = CourseStatus.Draft,
            Lessons = new List<Lesson> { new Lesson { IsDeleted = false } }
        };
        
        _mockRepo.Setup(repo => repo.GetCourseById(courseId))
            .ReturnsAsync(course);
        
        var result = await _service.PublishCourseAsync(courseId);
        
        Assert.True(result); 
        Assert.Equal(CourseStatus.Published, course.Status);
        
        _mockRepo.Verify(repo => repo.UpdateCourse(courseId, It.IsAny<Course>()), Times.Once);
    }
    
    [Fact]
    public async Task PublishCourse_WithoutLessons_ShouldFail()
    {
        var courseId = Guid.NewGuid();
        var course = new Course
        {
            Id = courseId,
            Status = CourseStatus.Draft,
            Lessons = new List<Lesson>()
        };

        _mockRepo.Setup(repo => repo.GetCourseById(courseId))
            .ReturnsAsync(course);

        
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _service.PublishCourseAsync(courseId));
    }
    
    [Fact]
    public async Task DeleteCourse_ShouldBeSoftDelete()
    {
        var courseId = Guid.NewGuid();
        
        _mockRepo.Setup(repo => repo.DeleteCourse(courseId))
            .ReturnsAsync(new Course { Id = courseId, IsDeleted = true });
        
        var result = await _service.DeleteCourseAsync(courseId);
        
        Assert.True(result);
        _mockRepo.Verify(repo => repo.DeleteCourse(courseId), Times.Once);
    }
}