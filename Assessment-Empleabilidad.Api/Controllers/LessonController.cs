using Assessment_Empleabilidad.Application.DTOs;
using Assessment_Empleabilidad.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Assessment_Empleabilidad.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LessonController : ControllerBase
{
    private readonly ILessonService _lessonService;
    private readonly ILogger<LessonController> _logger;

    public LessonController(ILessonService lessonService, ILogger<LessonController> logger)
    {
        _lessonService = lessonService;
        _logger = logger;
    }

    // GET: api/Lesson/course/{courseId}
    [HttpGet("course/{courseId:guid}")]
    public async Task<IActionResult> GetByCourse(Guid courseId)
    {
        var lessons = await _lessonService.GetLessonsByCourseIdAsync(courseId);
        return Ok(lessons);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var lesson = await _lessonService.GetLessonByIdAsync(id);
        if (lesson == null) return NotFound("Lección no encontrada");
        return Ok(lesson);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] LessonDtos.LessonCreateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var createdLesson = await _lessonService.AddLessonAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = createdLesson.Id }, createdLesson);
        }
        catch (ArgumentException ex) // Ej: Curso no existe
        {
            return BadRequest(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creando lección");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] LessonDtos.LessonUpdateDto dto)
    {
        if (id != dto.Id) return BadRequest("ID Mismatch");
        
        var updated = await _lessonService.UpdateLessonAsync(dto);
        if (updated == null) return NotFound("Lección no encontrada");
        
        return Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var success = await _lessonService.DeleteLessonAsync(id);
        if (!success) return NotFound("Lección no encontrada");
        return NoContent();
    }

    // PATCH: api/Lesson/{courseId}/reorder
    // Body: { "lessonId": "...", "newOrder": 3 }
    [HttpPatch("{courseId:guid}/reorder")]
    public async Task<IActionResult> Reorder(Guid courseId, [FromBody] LessonDtos.LessonReorderDto dto)
    {
        if (dto.NewOrder <= 0) return BadRequest("El orden debe ser mayor a 0");

        try
        {
            var result = await _lessonService.ReorderLessonAsync(courseId, dto.LessonId, dto.NewOrder);
            
            if (!result) return NotFound("Lección o Curso no encontrado");
            
            return Ok(new { Message = "Lecciones reordenadas correctamente" });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reordenando lecciones");
            return StatusCode(500, "Error interno del servidor");
        }
    }
}