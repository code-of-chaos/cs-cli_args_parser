// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System;
using System.Threading.Tasks;

namespace CodeOfChaos.CliArgsParser.Generators.Sample;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[CliArgsCommand("example")]
[CliArgsDescription("This is a test command")]
public partial class ExampleCommand : ICommand<CommandParameters> {
    public async Task ExecuteAsync(CommandParameters parameters) {
        // Log the mandatory "TestValue"
        Console.WriteLine($"TestValue: {parameters.TestValue}");

        // Check if verbose mode is enabled
        if (parameters.Verbose) {
            Console.WriteLine("Verbose mode is enabled.");
        }

        // Log the optional "OptionalTestValue", using its default value if not provided
        Console.WriteLine($"OptionalTestValue: {parameters.OptionalTestValue ?? "default value"}");

        // Log the optional "OptionalTestOtherValue", or tell if it's null
        Console.WriteLine(!string.IsNullOrEmpty(parameters.OptionalTestOtherValue)
            ? $"OptionalTestOtherValue: {parameters.OptionalTestOtherValue}"
            : "OptionalTestOtherValue is not provided."
        );

        // Log all values in the array "OptionalTestOtherElseValue"
        if (parameters.OptionalTestOtherElseValue.Length > 0) {
            Console.WriteLine("OptionalTestOtherElseValue:");
            foreach (string value in parameters.OptionalTestOtherElseValue) {
                Console.WriteLine($" - {value}");
            }
        }
        else {
            Console.WriteLine("OptionalTestOtherElseValue is empty.");
        }

        // Simulate some asynchronous work
        await Task.Delay(100); // Example of an asynchronous operation

        Console.WriteLine("Command executed successfully.");
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
