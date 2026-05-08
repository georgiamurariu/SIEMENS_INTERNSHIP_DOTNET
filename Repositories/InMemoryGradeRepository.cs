using Siemens.Internship2026.GradeBook.Interfaces;
using Siemens.Internship2026.GradeBook.Models;

namespace Siemens.Internship2026.GradeBook.Repositories;

public sealed class InMemoryGradeRepository : IGradeRepository
{
    private readonly IReadOnlyList<Grade> _grades = new List<Grade>
    {
        new() { Id = 1, Value = 9.50m, IsActive = true },
        new() { Id = 2, Value = 8.75m, IsActive = true },
        new() { Id = 3, Value = 7.25m, IsActive = true },
        new() { Id = 4, Value = 5.00m, IsActive = false },
        new() { Id = 5, Value = 4.75m, IsActive = true },
        new() { Id = 6, Value = 10.00m, IsActive = true }
    };

    public Task<Grade?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var grade = _grades.FirstOrDefault(grade => grade.Id == id);
        return Task.FromResult<Grade?>(grade);
    }

    public Task<IReadOnlyCollection<Grade>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<Grade> grades = _grades.ToList();
        return Task.FromResult(grades);
    }
}