namespace Siemens.Internship2026.GradeBook.Models;

public sealed record GradeStatistics
{
    public int TotalCount { get; init; }
    public decimal AverageValue { get; init; }
    public DateTime RetrievedAtUtc { get; init; }
}
