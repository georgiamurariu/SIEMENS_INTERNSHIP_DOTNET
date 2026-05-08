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
    private readonly IGradeRepository _gradeRepository;
    private readonly IGradeStatisticsService _statisticsService;
    private readonly ILogger<GradesController> _logger;

    public GradesController(
        IGradeRepository gradeRepository,
        IGradeStatisticsService statisticsService,
        ILogger<GradesController> logger)
    {
        _gradeRepository = gradeRepository;
        _statisticsService = statisticsService;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(GradeListResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<GradeListResponse>> GetAll(CancellationToken cancellationToken)
    {
        _logger.LogInformation("GET api/grades called");

        var grades = await _gradeRepository.GetAllAsync(cancellationToken);
        var retrievedAtUtc = DateTime.UtcNow;
        var statistics = _statisticsService.Calculate(grades, retrievedAtUtc);

        var response = new GradeListResponse
        {
            Data = grades,
            Statistics = statistics
        };

        _logger.LogInformation(
            "Returning {TotalCount} grades, average value: {AverageValue}",
            statistics.TotalCount,
            statistics.AverageValue);

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

        var grade = await _gradeRepository.GetByIdAsync(id, cancellationToken);
        if (grade is null)
        {
            return NotFound($"Grade with Id {id} was not found.");
        }

        return Ok(grade);
    }
}
