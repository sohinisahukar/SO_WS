---
mode: 'agent'
tools: ['codebase', 'githubRepo']
description: 'Automatically correct REST API controllers to follow best practices'
---

# REST API Controller Auto-Correction

**TASK:** Analyze the selected REST API controller code and automatically rewrite it to follow all REST API best practices and conventions. Return the corrected code with explanations.

## Auto-Fix These Issues:

### Controller Structure
- Add **[ApiController]** attribute
- Fix **routing** to `[Route("api/[controller]")]`
- Add **[Produces("application/json")]** attribute
- Ensure **proper inheritance** from ControllerBase
- Add **dependency injection** with null checks
- Add **ILogger** if missing

### HTTP Methods & Status Codes
- **GET**: Fix to return 200 (OK), 404 (Not Found), 400 (Bad Request)
- **POST**: Fix to return 201 (Created), 400 (Validation), 409 (Conflict)
- **PUT**: Fix to return 200 (OK), 404 (Not Found), 400 (Bad Request)
- **DELETE**: Fix to return 204 (No Content), 404 (Not Found)
- Add **[ProducesResponseType]** attributes for all possible responses

### Action Methods
- Add proper **HTTP verb attributes** ([HttpGet], [HttpPost], etc.)
- Fix **route templates** with constraints (`{id:int}`)
- Add **async/await** patterns if missing
- Fix **return types** to ActionResult<T>
- Add **input validation** for all parameters
- Add **ModelState validation** for DTOs
- Use **CreatedAtAction** for POST responses

### Request/Response DTOs
- Add **data annotations** for validation ([Required], [StringLength], etc.)
- Ensure **proper naming** conventions (PascalCase)
- Add **default values** where appropriate
- Create **separate DTOs** for requests vs responses

### Error Handling & Validation
- Add **parameter validation** (null checks, range validation)
- Add **ModelState.IsValid** checks
- Return **appropriate status codes**
- Add **logging statements** for operations
- Handle **service layer exceptions** properly

### Pagination & Filtering
- Add **pagination parameters** with defaults
- **Limit max page size** to prevent abuse
- Return **PagedResult<T>** for list endpoints
- Add **[FromQuery]** attributes for query parameters

## Output Format:

```csharp
// CORRECTED CONTROLLER CODE:
[Your corrected REST API controller code here - complete and ready to use]

// SUPPORTING DTOs (if created/modified):
[Any request/response DTOs that were created or corrected]

// CHANGES MADE:
// Added [ApiController] and proper routing attributes
// Line X: Fixed HTTP status code from 200 to 201 for POST
// Line Y: Added input validation for id parameter
// Added: ProducesResponseType attributes for OpenAPI documentation
// Added: Proper async/await pattern with ActionResult<T>
// Added: ModelState validation for request DTOs
// Added: ILogger dependency with proper logging statements
// Created: Separate request/response DTOs with validation attributes
```

## Requirements:
1. **Always return complete, compilable controller code**
2. **Follow RESTful conventions** exactly
3. **Add comprehensive input validation**
4. **Include proper HTTP status codes** for all scenarios
5. **Add OpenAPI documentation** attributes
6. **Implement proper error handling**
7. **Use async/await** for all I/O operations
8. **Create missing DTOs** with proper validation

## Critical Rules:
- **Never break existing API contracts** (routes, method signatures)
- **Always validate input parameters**
- **Return appropriate HTTP status codes**
- **Add comprehensive logging**
- **Follow REST naming conventions**
- **Implement proper pagination** for list endpoints
- **Add [FromBody], [FromQuery], [FromRoute]** attributes where needed
- **Ensure thread-safety** in all operations

## Standard Controller Template Applied:
```csharp
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class EntityNameController : ControllerBase
{
    private readonly IEntityNameService _service;
    private readonly ILogger<EntityNameController> _logger;
    
    // GET, POST, PUT, DELETE with proper status codes
    // Input validation and error handling
    // Async/await patterns
    // Comprehensive logging
    // OpenAPI documentation attributes
}
```

**Goal:** Transform any controller code into production-ready REST API that follows enterprise standards, proper HTTP semantics, and comprehensive error handling.