# CodeOfChaos.CliArgsParser

## Overview

CodeOfChaos.CliArgsParser is a library designed to simplify the process of creating command-line applications in .NET
projects. It provides developers with attributes, structures, and tools to quickly define, parse, and execute CLI
commands with minimal boilerplate code. The library is focused on flexibility, clarity, and ease of use, utilizing C#'s
powerful features like attributes and partial classes.

---

## Features

- **Easy Command Definition**: Define commands using attributes like `[CliArgsCommand]` and `[CliArgsParameter]`
  directly in your application.
- **Automatic Parsing**: Command-line arguments are parsed and mapped to parameters automatically.
- **Description Metadata**: Add meaningful descriptions to commands and parameters using `[CliArgsDescription]` for
  better documentation.
- **Support for Flags and Complex Types**: Easily handle flags, strings, arrays, and even optional/default values.
- **Asynchronous Execution**: Built-in support for asynchronous methods to handle complex operations.

---

## Getting Started

To use CodeOfChaos.CliArgsParser in your project, follow these steps:

### 1. Install the Library

Add the library to your project via NuGet:

```bash
dotnet add package CodeOfChaos.CliArgsParser
```

---

### 2. Define a Command

Commands are defined as partial classes and annotated with metadata. For example:

```csharp
[CliArgsCommand("example")]
[CliArgsDescription("This is a test command")]
public partial class ExampleCommand : ICommand<ExampleCommandParameters> {
    public async Task ExecuteAsync(ExampleCommandParameters parameters) {
        Console.WriteLine($"Running example command with TestValue: {parameters.TestValue}");
        
        if (parameters.Verbose) {
            Console.WriteLine("Verbose mode enabled.");
        }
        
        await Task.Delay(100); // Simulating async work
    }
}
```

---

### 3. Define Parameters

Parameters for a command are defined using partial structs with annotated attributes:

```csharp
public readonly partial struct ExampleCommandParameters : IParameters
{
    [CliArgsParameter("test", "t")]
    [CliArgsDescription("A required test parameter")]
    public required string TestValue { get; init; }

    [CliArgsParameter("verbose", "v", ParameterType.Flag)]
    [CliArgsDescription("Enable verbose logging")]
    public bool Verbose { get; init; }
    
    [CliArgsParameter("optional-parameter", "o")]
    [CliArgsDescription("An optional parameter")]
    public string? OptionalValue { get; init; } = "default";
}
```

---

### 4. Register and Run Commands

Typically, this is performed in your `Program.cs` or entry point:

```csharp
using CodeOfChaos.CliArgsParser;

CliArgsParser parser = CliArgsBuilder.CreateFromConfig(
    config => {
        config.AddCommand<ExampleCommand>();
    }
).Build();

await parser.ParseAsync(args);
```