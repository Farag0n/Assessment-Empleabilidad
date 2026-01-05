using Assessment_Empleabilidad.Application.DTOs;
using Assessment_Empleabilidad.Application.Interfaces;
using Assessment_Empleabilidad.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Assessment_Empleabilidad.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Protege todos los endpoints por defecto
public class CourseController : ControllerBase
{
    private readonly ICourseService _courseService;
    private readonly ILogger<CourseController> _logger;

    public CourseController(ICourseService courseService, ILogger<CourseController> logger)
    {
        _courseService = courseService;
        _logger = logger;
    }

    // GET: api/Course/search?searchTerm=react&status=Published&page=1&pageSize=10
    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] string? searchTerm, 
        [FromQuery] CourseStatus? status, 
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 10)
    {
        try
        {
            var result = await _courseService.SearchCoursesAsync(searchTerm, status, page, pageSize);
            
            // Retornamos una estructura paginada estándar
            return Ok(new
            {
                Data = result.Items,
                TotalCount = result.TotalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(result.TotalCount / (double)pageSize)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error buscando cursos");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var course = await _courseService.GetCourseByIdAsync(id);
        if (course == null) return NotFound("Curso no encontrado");
        return Ok(course);
    }

    // GET: api/Course/{id}/summary
    [HttpGet("{id:guid}/summary")]
    public async Task<IActionResult> GetSummary(Guid id)
    {
        var summary = await _courseService.GetCourseSummaryAsync(id);
        if (summary == null) return NotFound("Curso no encontrado");
        return Ok(summary);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CourseDtos.CourseCreateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var createdCourse = await _courseService.AddCourseAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = createdCourse.Id }, createdCourse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creando curso");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CourseDtos.CourseUpdateDto dto)
    {
        if (id != dto.Id) return BadRequest("El ID de la URL no coincide con el del cuerpo");
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var updated = await _courseService.UpdateCourseAsync(dto);
        if (updated == null) return NotFound("Curso no encontrado");

        return Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var success = await _courseService.DeleteCourseAsync(id);
        if (!success) return NotFound("Curso no encontrado");
        return NoContent(); // 204 No Content
    }

    // PATCH: api/Course/{id}/publish
    [HttpPatch("{id:guid}/publish")]
    public async Task<IActionResult> Publish(Guid id)
    {
        try
        {
            var success = await _courseService.PublishCourseAsync(id);
            if (!success) return NotFound("Curso no encontrado");
            return Ok(new { Message = "Curso publicado exitosamente" });
        }
        catch (InvalidOperationException ex)
        {
            // Capturamos la regla de negocio: "No se puede publicar sin lecciones"
            return BadRequest(new { Message = ex.Message });
        }
    }

    // PATCH: api/Course/{id}/unpublish
    [HttpPatch("{id:guid}/unpublish")]
    public async Task<IActionResult> Unpublish(Guid id)
    {
        var success = await _courseService.UnpublishCourseAsync(id);
        if (!success) return NotFound("Curso no encontrado");
        return Ok(new { Message = "Curso devuelto a borrador" });
    }
}