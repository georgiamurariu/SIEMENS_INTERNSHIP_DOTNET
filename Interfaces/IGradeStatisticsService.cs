using Siemens.Internship2026.GradeBook.Models;

namespace Siemens.Internship2026.GradeBook.Interfaces;

public interface IGradeStatisticsService
{
    GradeStatistics Calculate(IReadOnlyCollection<Grade> grades, DateTime retrievedAtUtc);
}
