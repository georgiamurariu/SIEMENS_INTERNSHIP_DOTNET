using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Siemens.Internship2026.GradeBook.Interfaces;
using Siemens.Internship2026.GradeBook.Models;

namespace Siemens.Internship2026.GradeBook.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class GradesController : ControllerBase
{
    private readonly IGradeService _gradeService;
    private readonly IGradeStatisticsService _statisticsService;
    private readonly ILogger<GradesController> _logger;

    public GradesController(
        IGradeService gradeService,
        IGradeStatisticsService statisticsService,
        ILogger<GradesController> logger)
    {
        _gradeService = gradeService;
        _statisticsService = statisticsService;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(GradeListResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<GradeListResponse>> GetAll(CancellationToken cancellationToken)
    {
        _logger.LogInformation("GET api/grades called");

        var grades = await _gradeService.GetAllActiveGradesAsync(cancellationToken);
        var retrievedAtUtc = DateTime.UtcNow;
        var statistics = _statisticsService.Calculate(grades, retrievedAtUtc);

        var response = new GradeListResponse
        {
            Data = grades,
            Statistics = statistics
        };

        return Ok(response);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Grade), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Grade>> GetById(int id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("GET api/grades/{GradeId} called", id);

        if (id <= 0)
        {
            return BadRequest("Id must be a positive integer.");
        }

        var grade = await _gradeService.GetActiveGradeByIdAsync(id, cancellationToken);

        if (grade is null)
        {
            return NotFound($"Grade with Id {id} was not found.");
        }

        return Ok(grade);
    }

    [HttpGet("passing")]
    [ProducesResponseType(typeof(IReadOnlyCollection<Grade>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyCollection<Grade>>> GetFirstPassingActiveGrades(
        [FromQuery(Name = "n")] int count,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("GET api/grades/passing?n={Count} called", count);

        if (count <= 0)
        {
            return BadRequest("N must be a positive integer.");
        }

        var grades = await _gradeService.GetFirstPassingActiveGradesAsync(count, cancellationToken);

        return Ok(grades);
    }
}