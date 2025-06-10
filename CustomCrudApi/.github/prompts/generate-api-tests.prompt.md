---
mode: 'agent'
tools: ['codebase', 'terminal']
description: 'Create unit and integration test folders and test files in a .NET 8 Web API project using xUnit and Moq'
---

You are a helpful assistant tasked with adding a testing structure to an existing .NET 8 Web API project.

## Goal:
Create a `Tests/` folder **inside the existing project folder**, not a separate project.

### Structure:
RawCrudApi/
├── Controllers/
├── Models/
├── Services/
├── Tests/
│ ├── Unit/
│ │ └── BookServiceTests.cs
│ └── Integration/
│ └── BookApiTests.cs

markdown
Copy
Edit

---

### Requirements:

1. **Unit Tests** (`Tests/Unit/BookServiceTests.cs`):
   - Use `xUnit` and `Moq`
   - Test methods in `BookService.cs`
     - `Get()`, `Get(id)`, `Create()`, `Update()`, `Delete()`
   - Mock `IMongoCollection<Book>` and use in-memory book data

2. **Integration Tests** (`Tests/Integration/BookApiTests.cs`):
   - Use `WebApplicationFactory<Program>`
   - Create an HTTP client and:
     - `POST` a new book
     - `GET` it by ID
     - Assert response status and payload

3. Use `[Fact]` attributes and good test naming conventions (e.g. `Get_ReturnsAllBooks`)

4. Do **not** create a separate `.csproj` for the test files. Everything stays in the main project folder.

5. Register test-related NuGet packages via CLI if not already added:
   ```bash
   dotnet add package xunit
   dotnet add package Moq
   dotnet add package Microsoft.AspNetCore.Mvc.Testing