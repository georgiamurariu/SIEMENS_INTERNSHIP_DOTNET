using System.Text.Json;
using Siemens.Internship2026.GradeBook.Interfaces;
using Siemens.Internship2026.GradeBook.Models;

namespace Siemens.Internship2026.GradeBook.Repositories;

public sealed class ExternalGradeRepository : IGradeRepository
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ExternalGradeRepository> _logger;

    public ExternalGradeRepository(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<ExternalGradeRepository> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<Grade>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var endpoint = _configuration["GradeData:Endpoint"];

        if (string.IsNullOrWhiteSpace(endpoint))
        {
            throw new InvalidOperationException("Grade data endpoint is not configured.");
        }

        try
        {
            var json = await _httpClient.GetStringAsync(endpoint, cancellationToken);

            using var document = JsonDocument.Parse(json);
            var root = document.RootElement;

            JsonElement gradesElement;

            if (root.ValueKind == JsonValueKind.Array)
            {
                gradesElement = root;
            }
            else if (root.ValueKind == JsonValueKind.Object &&
                     root.TryGetProperty("data", out var dataElement))
            {
                gradesElement = dataElement;
            }
            else if (root.ValueKind == JsonValueKind.Object &&
                     root.TryGetProperty("grades", out var gradesPropertyElement))
            {
                gradesElement = gradesPropertyElement;
            }
            else if (root.ValueKind == JsonValueKind.Object &&
                     root.TryGetProperty("items", out var itemsElement))
            {
                gradesElement = itemsElement;
            }
            else
            {
                throw new JsonException("The external endpoint response does not contain a valid grades collection.");
            }

            var grades = JsonSerializer.Deserialize<List<Grade>>(
                gradesElement.GetRawText(),
                JsonOptions);

            return grades ?? new List<Grade>();
        }
        catch (HttpRequestException exception)
        {
            _logger.LogError(exception, "Failed to retrieve grades from external endpoint.");
            throw;
        }
        catch (JsonException exception)
        {
            _logger.LogError(exception, "Failed to deserialize grades from external endpoint.");
            throw;
        }
    }

    public async Task<Grade?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var grades = await GetAllAsync(cancellationToken);

        return grades.FirstOrDefault(grade => grade.Id == id);
    }
}