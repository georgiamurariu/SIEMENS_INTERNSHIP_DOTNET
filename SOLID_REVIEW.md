# SOLID Review - Siemens.Internship2026.GradeBook

This document describes the SOLID issues found in the original project and the fixes applied during refactoring.

## 1. Single Responsibility Principle violation

### Principle violated
Single Responsibility Principle (SRP)

### Original location
- File: `Controllers/ItemController.cs`
- Method: `GetAll()`
- Lines: 17-40

### Why it is a violation
`ItemController.GetAll()` handled multiple responsibilities at the same time:

- received the HTTP request;
- read data from the repository;
- calculated statistics (`totalCount`, `averageValue`);
- created the API response;
- wrote log messages directly to the console.

A controller should mainly coordinate the request/response flow. Business logic such as statistics calculation should be moved to a separate service.

### Fix applied
- Renamed `ItemController` to `GradesController`.
- Created `IGradeStatisticsService` in `Interfaces/IGradeStatisticsService.cs`.
- Created `GradeStatisticsService` in `Services/GradeStatisticsService.cs`.
- Moved statistics calculation to `GradeStatisticsService.Calculate()`.
- The controller now only calls the repository and the statistics service.

New relevant locations:

- `Controllers/GradesController.cs`, method `GetAll()`, lines 26-48
- `Services/GradeStatisticsService.cs`, method `Calculate()`, lines 8-18

---

## 2. Open/Closed Principle violation

### Principle violated
Open/Closed Principle (OCP)

### Original location
- File: `Controllers/ItemController.cs`
- Method: `GetAll()`
- Lines: 25-37

### Why it is a violation
The statistics were calculated directly inside the controller. If the application later needed more statistics, such as minimum grade, maximum grade, failed grades count, or median value, the controller would have to be modified every time.

This makes the controller less extensible and couples it to business rules.

### Fix applied
- Introduced the abstraction `IGradeStatisticsService`.
- The controller depends on the abstraction, not on the statistics implementation.
- Future statistics changes can be made in `GradeStatisticsService` or in another implementation without changing the controller request logic.

New relevant locations:

- `Interfaces/IGradeStatisticsService.cs`, lines 5-8
- `Services/GradeStatisticsService.cs`, lines 6-18
- `Controllers/GradesController.cs`, lines 12-23 and 32-34

---

## 3. Dependency Inversion Principle violation

### Principle violated
Dependency Inversion Principle (DIP)

### Original location
- File: `Controllers/ItemController.cs`
- Methods: `GetAll()` and `GetById()`
- Lines: 20, 28, 45, 49, 56

### Why it is a violation
The controller used `Console.WriteLine()` directly for logging. This couples the controller to a concrete output mechanism and makes logging harder to configure, replace, or test.

ASP.NET Core already provides logging through the `ILogger<T>` abstraction, so the controller should depend on that abstraction instead of using `Console` directly.

### Fix applied
- Removed all `Console.WriteLine()` calls.
- Injected `ILogger<GradesController>` through the controller constructor.
- Replaced console logging with `_logger.LogInformation(...)`.

New relevant location:

- `Controllers/GradesController.cs`, lines 14, 19, 30, 42-45, 56

---

## 4. Dependency registration error

### Principle violated
Dependency Inversion Principle (DIP)

### Original location
- File: `Program.cs`
- Lines: 3-9

### Why it is a violation
`ItemController` depended on `IItemReader`, but the application did not register any implementation for that interface in the dependency injection container.

This would cause a runtime error when the API tries to create the controller, because ASP.NET Core cannot resolve the required dependency.

### Fix applied
- Renamed `IItemReader` to `IGradeRepository` for clearer domain meaning.
- Renamed `ItemRepository` to `InMemoryGradeRepository`.
- Registered dependencies in `Program.cs`:
  - `IGradeRepository -> InMemoryGradeRepository`
  - `IGradeStatisticsService -> GradeStatisticsService`

New relevant location:

- `Program.cs`, lines 7-9

---

## 5. Liskov Substitution Principle risk caused by inheritance design

### Principle violated
Liskov Substitution Principle (LSP)

### Original location
- File: `Repositories/ItemRepository.cs`
- Lines: 8-19

### Why it is a violation
The original repository exposed its internal list as `protected` and marked its methods as `virtual` even though there was no real inheritance scenario in the project.

A derived repository could override `GetAllAsync()` or change `_items` in a way that breaks the expected behavior, for example by returning inactive items or by changing repository state unexpectedly.

### Fix applied
- Replaced `protected` mutable fields with a `private readonly` collection.
- Removed unnecessary inheritance points by making `InMemoryGradeRepository` `sealed`.
- Kept access to data only through the `IGradeRepository` abstraction.

New relevant location:

- `Repositories/InMemoryGradeRepository.cs`, lines 6-30

---

## 6. Naming and domain clarity issue

### Principle involved
Clean code / maintainability. This is not a direct SOLID principle, but it affects readability and design quality.

### Original location
- File: `Models/Item.cs`, class `Item`, lines 3-8
- File: `Controllers/ItemController.cs`, class `ItemController`, line 8
- File: `Interfaces/IItemReader.cs`, interface `IItemReader`, lines 5-9
- File: `Repositories/ItemRepository.cs`, class `ItemRepository`, line 6

### Why it is a problem
The project is called `GradeBook`, but the domain object was named `Item`. This name is too generic and does not explain the business purpose of the API.

### Fix applied
Renamed the domain classes and interfaces:

- `Item` -> `Grade`
- `ItemController` -> `GradesController`
- `IItemReader` -> `IGradeRepository`
- `ItemRepository` -> `InMemoryGradeRepository`

New relevant locations:

- `Models/Grade.cs`
- `Controllers/GradesController.cs`
- `Interfaces/IGradeRepository.cs`
- `Repositories/InMemoryGradeRepository.cs`

---

## 7. Response model clarity issue

### Principle involved
Single Responsibility Principle (SRP) and maintainability.

### Original location
- File: `Controllers/ItemController.cs`
- Method: `GetAll()`
- Lines: 30-39

### Why it is a problem
The original response was built using an anonymous object inside the controller. Anonymous response shapes are harder to reuse, document, validate, and test.

### Fix applied
Created explicit response models:

- `GradeListResponse`
- `GradeStatistics`

New relevant locations:

- `Models/GradeListResponse.cs`, lines 3-7
- `Models/GradeStatistics.cs`, lines 3-8
- `Controllers/GradesController.cs`, lines 36-40

---

## Principles reviewed with no direct violation found

### Interface Segregation Principle (ISP)
No direct ISP violation was found in the original code. The original `IItemReader` interface only contained read operations, and the controller used both methods.

