---
mode: 'agent'
tools: ['codebase', 'githubRepo']
description: 'Generate secure C# code following security best practices'
---

You are generating C# code with security as the top priority. Always follow these security practices:

## SQL Injection Prevention
* ALWAYS use parameterized queries with `@parameter` syntax
* Use Entity Framework properly: `.Where(x => x.Id == id)` 
* NEVER concatenate user input: `$"SELECT * FROM table WHERE id = '{userInput}'"` is forbidden

## Input Validation
```csharp
public class CreateUserRequest
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;
}

// Always validate in controllers
if (!ModelState.IsValid) 
    return BadRequest(ModelState);
```

## Authentication & Authorization
```csharp
[Authorize(Roles = "Admin,Manager")]
[HttpDelete("{id:int}")]
public async Task<IActionResult> DeleteUser(int id)
{
    // Additional business logic authorization
    if (!await _userService.CanUserDeleteAsync(User.GetUserId(), id))
        return Forbid();
        
    await _userService.DeleteUserAsync(id);
    return NoContent();
}
```

## Sensitive Data Handling
* Never log passwords, tokens, API keys, or PII
* Use `[JsonIgnore]` attribute for sensitive properties in DTOs
* Store secrets in configuration (appsettings.json, Azure Key Vault)
* Use HTTPS for all API endpoints

## Secure Error Handling
```csharp
try 
{
    var result = await _service.ProcessAsync(request);
    return Ok(result);
}
catch (ValidationException ex)
{
    _logger.LogWarning("Validation failed for request {RequestId}", requestId);
    return BadRequest(ex.Message);
}
catch (Exception ex)
{
    _logger.LogError(ex, "Unexpected error processing request {RequestId}", requestId);
    return Problem("An error occurred processing your request");
}
```

When generating any C# code, prioritize security over convenience. Always include proper validation, authorization checks, and secure data handling.