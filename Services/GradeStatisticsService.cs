using Siemens.Internship2026.GradeBook.Interfaces;
using Siemens.Internship2026.GradeBook.Models;

namespace Siemens.Internship2026.GradeBook.Services;

public sealed class GradeStatisticsService : IGradeStatisticsService
{
    public GradeStatistics Calculate(IReadOnlyCollection<Grade> grades, DateTime retrievedAtUtc)
    {
        ArgumentNullException.ThrowIfNull(grades);

        return new GradeStatistics
        {
            TotalCount = grades.Count,
            AverageValue = grades.Count == 0 ? 0 : grades.Average(grade => grade.Value),
            RetrievedAtUtc = retrievedAtUtc
        };
    }
}
