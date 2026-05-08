using Siemens.Internship2026.GradeBook.Interfaces;
using Siemens.Internship2026.GradeBook.Models;

namespace Siemens.Internship2026.GradeBook.Services;

public sealed class GradeService : IGradeService
{
    private const decimal PassingGradeValue = 5m;

    private readonly IGradeRepository _gradeRepository;

    public GradeService(IGradeRepository gradeRepository)
    {
        _gradeRepository = gradeRepository;
    }

    public async Task<IReadOnlyCollection<Grade>> GetAllActiveGradesAsync(
        CancellationToken cancellationToken = default)
    {
        var grades = await _gradeRepository.GetAllAsync(cancellationToken);

        return grades
            .Where(grade => grade.IsActive)
            .ToList();
    }

    public async Task<Grade?> GetActiveGradeByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var grade = await _gradeRepository.GetByIdAsync(id, cancellationToken);

        if (grade is null || !grade.IsActive)
        {
            return null;
        }

        return grade;
    }

    public async Task<IReadOnlyCollection<Grade>> GetFirstPassingActiveGradesAsync(
        int count,
        CancellationToken cancellationToken = default)
    {
        var grades = await _gradeRepository.GetAllAsync(cancellationToken);

        return grades
            .Where(grade => grade.IsActive)
            .Where(grade => grade.Value >= PassingGradeValue)
            .Take(count)
            .ToList();
    }
}