# Requirement III - Service Layer

This branch implements requirement III for the Siemens Internship 2026 GradeBook project.

## Requirement

Introduce a service layer that encapsulates the business logic.

Within this layer, implement a filter that retrieves only the first N grades that meet both criteria:

- the grade is a passing grade, meaning its value is greater than or equal to 5;
- the grade is active.

N is a parameter provided by the user.

## Implemented solution

A new service layer was introduced through:

- `Interfaces/IGradeService.cs`
- `Services/GradeService.cs`

The business logic was moved from the controller/repository into `GradeService`.

The service contains the following methods:

- `GetAllActiveGradesAsync()`
- `GetActiveGradeByIdAsync(int id)`
- `GetFirstPassingActiveGradesAsync(int count)`

The filtering logic for requirement III is implemented in:

```csharp
GradeService.GetFirstPassingActiveGradesAsync(int count)

The method filters grades by:

grade.IsActive
grade.Value >= 5
Take(count)
New endpoint
GET /api/grades/passing?n=2

Example:

http://localhost:5030/api/grades/passing?n=2

This returns the first 2 grades that are both active and passing.

Validation

If n is less than or equal to 0, the API returns:

400 Bad Request
How to run
dotnet restore
dotnet build
dotnet run
Test endpoints
GET http://localhost:5030/api/grades
GET http://localhost:5030/api/grades/1
GET http://localhost:5030/api/grades/passing?n=2

Apoi dai push pentru cerința 3:

```bash
git add README.md
git commit -m "Add README for requirement 3"
git push -u origin cerinta_3