// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.CliArgsParser.Contracts;
using CodeOfChaos.CliArgsParser.Attributes;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace CodeOfChaos.CliArgsParser.Generators.Sample;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[CliArgsCommand("test")]
[CliArgsDescription("This is a test command")]
public class Command : ICommand<CommandParameters> {
    public Task ExecuteAsync(CommandParameters parameters) {
        throw new NotImplementedException();
    }
}

public readonly partial struct CommandParameters : IParameters {
    [CliArgsParameter("test", "t")]
    [CliArgsDescription("This is a test parameter")]
    public required string TestValue { get; init; }
    
    [CliArgsParameter("verbose", "v", ParameterType.Flag)]
    [CliArgsDescription("This is a verbose parameter")]
    public required bool Verbose { get; init; }
    
    [CliArgsParameter("test-optional", "to")]
    [CliArgsDescription("This is a test parameter")]
    public string? OptionalTestValue { get; init; } = "test";
    
    [CliArgsParameter("test-other-optional", "too")]
    [CliArgsDescription("This is a test parameter")]
    public string? OptionalTestOtherValue { get; init; }


    [CliArgsParameter("test-other-optional-else", "too")]
    [CliArgsDescription("This is a test parameter")]
    public string[] OptionalTestOtherElseValue { get; init; }
}

// Auto generated
// public readonly partial struct CommandParameters() {
//     public static CommandParameters FromRegistry(CommandInputRegistry registry) =>
//         new() {
//             TestValue = registry.GetParameter<string>("test"),
//             Verbose = registry.GetParameter<bool>("verbose"),
//             OptionalTestValue = registry.GetOptionalParameter<string>("test-optional") ?? "test",
//             OptionalTestOtherValue = registry.GetOptionalParameter<string>("test-optional") ?? default,
//             OptionalTestOtherElseValue = registry.GetOptionalParameter<string[]>("test-other-optional-else") ?? [],
//         };
// }

// Helper class to do a lot of the manipulation
public class CommandInputRegistry {
    public bool TryGetParameter<T>(string key, [NotNullWhen(true)] out T? value) => throw new NotImplementedException();
    public T GetParameter<T>(string key) => throw new NotImplementedException();
    public T? GetOptionalParameter<T>(string key) => default;
}