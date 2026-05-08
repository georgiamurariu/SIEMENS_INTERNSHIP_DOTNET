namespace Siemens.Internship2026.GradeBook.Models;

public sealed record Grade
{
    public int Id { get; init; }
    public decimal Value { get; init; }
    public bool IsActive { get; init; } = true;
}
