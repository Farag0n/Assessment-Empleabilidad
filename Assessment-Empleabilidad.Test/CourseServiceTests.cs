using Assessment_Empleabilidad.Application.Services;
using Assessment_Empleabilidad.Domain.Entities;
using Assessment_Empleabilidad.Domain.Enums;
using Assessment_Empleabilidad.Domain.Interfaces;
using Moq;
using Xunit;

namespace Assessment_Empleabilidad.Tests;

public class CourseServiceTests
{
    private readonly Mock<ICourseRepository> _mockRepo;//repositorio simulado
    private readonly CourseService _service; //el servicio que se va a testear

    public CourseServiceTests()
    {
        _mockRepo = new Mock<ICourseRepository>();
        _service = new CourseService(_mockRepo.Object);
    }

    //Fact es un test que se ejecuta sin parametros
    //Para un test que se ejecute con parametros se usa [Theory]
    //validar que un curso valido se puede publicar de forma correcta
    [Fact]
    public async Task PublishCourse_WithLessons_ShouldSucceed()
    {
        //se crear un curso para testear
        var courseId = Guid.NewGuid();
        var course = new Course
        {
            Id = courseId,
            Status = CourseStatus.Draft,
            Lessons = new List<Lesson> { new Lesson { IsDeleted = false } }
        };

        //cuando el servicio llame a GetCouseById() el servico no usara la capa de infrastructura
        _mockRepo.Setup(repo => repo.GetCourseById(courseId))
            .ReturnsAsync(course);
        
        //ejeccucion del test
        var result = await _service.PublishCourseAsync(courseId);
        
        //verificaciones del test
        Assert.True(result); 
        Assert.Equal(CourseStatus.Published, course.Status);
        
        //verificar que se llamo a update course
        _mockRepo.Verify(repo => repo.UpdateCourse(courseId, It.IsAny<Course>()), Times.Once);
    }
    
    //publicar curso sin lecciones (debe fallar)
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


        //act + assert combinados
        //esto prueba una regla de negocio "no se puede publicar un curso sin lecciones"
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _service.PublishCourseAsync(courseId));
    }

    //validar el borrado logico
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