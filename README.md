# Requirement IV - Repository Refactoring

This branch implements requirement IV for the Siemens Internship 2026 GradeBook project.

This branch also includes requirement III, because the repository refactoring is used by the service layer.

## Requirement

Refactor the repository layer to replace the current in-memory data source with data fetched from an external endpoint.

External endpoint:

```text
https://gist.githubusercontent.com/ArdeleanTudor/8ea407832cd9794960e0e6bbd1319f6e/raw

mplemented solution

The previous in-memory repository was replaced with an external repository that retrieves grades through HTTP.

Added repository:

Repositories/ExternalGradeRepository.cs

The repository uses:

HttpClient
IConfiguration
ILogger<ExternalGradeRepository>

The external endpoint is configured in:

appsettings.json
{
  "GradeData": {
    "Endpoint": "https://gist.githubusercontent.com/ArdeleanTudor/8ea407832cd9794960e0e6bbd1319f6e/raw"
  }
}
Dependency injection change

The repository registration was changed from the in-memory implementation to the external implementation.

Before:

builder.Services.AddSingleton<IGradeRepository, InMemoryGradeRepository>();

After:

builder.Services.AddHttpClient<IGradeRepository, ExternalGradeRepository>();

This means the controller and service still depend on the abstraction IGradeRepository, but the concrete data source is now external.

Service layer usage

The service layer from requirement III remains responsible for business logic.

The external repository only retrieves data.

The filtering logic is still handled in:

GradeService.GetFirstPassingActiveGradesAsync(int count)

Filtering rules:

grade must be active;
grade value must be greater than or equal to 5;
only the first N matching grades are returned.
Endpoints
GET /api/grades
GET /api/grades/{id}
GET /api/grades/passing?n=2

Examples:

http://localhost:5030/api/grades
http://localhost:5030/api/grades/1
http://localhost:5030/api/grades/passing?n=2

How to run
dotnet restore
dotnet build
dotnet run