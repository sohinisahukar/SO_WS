using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Microsoft.AspNetCore.TestHost")]
[assembly: InternalsVisibleTo("Microsoft.AspNetCore.Mvc.Testing")]

namespace CustomCrudApi.Tests;

public class TestProgram
{
    // This class exists to provide an entry point for tests
    // and avoid conflicts with the main Program.cs
}