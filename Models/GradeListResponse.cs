namespace Siemens.Internship2026.GradeBook.Models;

public sealed record GradeListResponse
{
    public IReadOnlyCollection<Grade> Data { get; init; } = Array.Empty<Grade>();
    public GradeStatistics Statistics { get; init; } = new();
}
