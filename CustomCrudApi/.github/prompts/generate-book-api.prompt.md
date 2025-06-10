---
mode: 'agent'
tools: ['githubRepo', 'codebase']
description: 'Generate C# code for a .NET 8 Web API with MongoDB CRUD functionality'
---

You are generating code for a simple .NET 8 Web API using C# and MongoDB Atlas.

The project has already been initialized with `dotnet new webapi`.

Your tasks:
1. Generate a `Book` model with properties: Id, Title, Author, Year.
2. Create a MongoDB configuration class to read settings from appsettings.json.
3. Create a `BookService` to handle CRUD operations using MongoDB.Driver.
4. Create a `BooksController` with REST endpoints:
   - GET /api/books (list all)
   - GET /api/books/{id}
   - POST /api/books
   - PUT /api/books/{id}
   - DELETE /api/books/{id}
5. Register all services inside Program.cs (`builder.Services.AddSingleton<BookService>()`)
6. Enable Swagger for easy testing.
7. Ensure correct namespaces and folders are used (`Models/`, `Services/`, `Controllers/`).
8. Optionally show how to call the API via Postman.

Make sure all classes and endpoints follow best practices and compile correctly.
