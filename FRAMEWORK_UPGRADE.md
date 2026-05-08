# Framework Upgrade - .NET 10

The project was upgraded from ASP.NET Core 8 to ASP.NET Core 10.

## Change applied

Updated the target framework in `Siemens.Internship2026.GradeBook.csproj`:

```xml
<TargetFramework>net10.0</TargetFramework>
```

## Validation

The project should be validated with:

```bash
dotnet restore
dotnet build
dotnet run
```

Expected result:

- restore succeeds;
- build succeeds without errors;
- the API starts successfully;
- `GET /api/grades` returns the grade list and statistics.