---
mode: 'agent'
tools: ['codebase']
description: 'Automatically correct C# code to follow Microsoft coding standards'
---

# C# Code Auto-Correction

**TASK:** Analyze the selected C# code and automatically rewrite it to follow all Microsoft coding standards and best practices. Return the corrected code with explanations.

## Auto-Fix These Issues:

### Naming Conventions
- Convert to **PascalCase**: Classes, Methods, Properties, Public Fields
- Convert to **camelCase**: Parameters, Local Variables  
- Add **underscore prefix** to private fields: `_fieldName`
- Make names **descriptive**: `GetUserById(int userId)` not `GetUser(int id)`

### Async/Await Patterns
- Add **Async suffix** to async methods
- Replace **blocking calls** (.Result, .Wait()) with proper await
- Add **ConfigureAwait(false)** in library code
- Fix **async void** to **async Task**

### Error Handling
- Add **input validation** with ArgumentException/ArgumentNullException
- Add **proper exception handling** with specific catch blocks
- Add **logging statements** for operations and errors
- Use **appropriate exception types**

### Dependency Injection
- Add **null checks** in constructors with ArgumentNullException
- Ensure **proper DI registration** comments
- Fix **lifetime issues**

### Code Structure
- **Extract large methods** into smaller ones
- Add **missing interfaces**
- Add **XML documentation** for public members
- **Organize using statements**
- Add **proper regions** if beneficial

## Output Format:

```csharp
// CORRECTED CODE:
[Your corrected C# code here - complete and ready to use]

// CHANGES MADE:
// Line X: Fixed naming convention - renamed 'getData' to 'GetDataAsync'
// Line Y: Added input validation for userId parameter
// Line Z: Added ConfigureAwait(false) to async call
// Line A: Added: ILogger dependency injection with null check
// Line B: Added: XML documentation for public methods
```

## Requirements:
1. **Always return complete, compilable code**
2. **Preserve original functionality** - only improve structure/standards
3. **Add missing dependencies** (ILogger, etc.) where needed
4. **Include brief explanation** of major changes
5. **Maintain existing business logic** exactly
6. **Follow the exact template structure** from the original standards document

## Critical Rules:
- **Never break existing functionality**
- **Always add proper error handling**
- **Include comprehensive input validation**
- **Use async/await patterns correctly**
- **Follow Microsoft naming conventions exactly**
- **Add logging for all public methods**

**Goal:** Transform any C# code into production-ready code that passes all code reviews and follows enterprise standards.