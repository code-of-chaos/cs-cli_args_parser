// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.CliArgsParser.Contracts;
using CodeOfChaos.CliArgsParser.Attributes;
using System;
using System.Threading.Tasks;

namespace CodeOfChaos.CliArgsParser.Generators.Sample;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[CliArgsCommand("test")]
[CliArgsDescription("This is a test command")]
public partial class ExampleCommand : ICommand<CommandParameters> {
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
    
    [CliArgsParameter("test-other-optional-else", "tooe")]
    [CliArgsDescription("This is a test parameter")]
    public string[] OptionalTestOtherElseValue { get; init; } = [];
}