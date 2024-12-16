// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.CliArgsParser.Contracts;

namespace CodeOfChaos.CliArgsParser;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class CliArgsBuilder(CliArgsBuilderConfig config) {
    private readonly Dictionary<string, (CommandData, IHasInitializeAsync)> _commands = new();

    // -----------------------------------------------------------------------------------------------------------------
    // Constructors
    // -----------------------------------------------------------------------------------------------------------------
    public static CliArgsBuilder CreateFromConfig(Action<CliArgsBuilderConfig> configuration) {
        CliArgsBuilderConfig config = new();
        configuration(config);
        return new CliArgsBuilder(config);
    }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public CliArgsParser Build() {
        while (config.Commands.TryPop(out Type? commandType)) {
            if (Activator.CreateInstance(commandType) is not IHasCommandData { CommandData: var commandData } command) throw new Exception($"Command type {commandType.Name} does not implement IHasCommandData.");
            if (command is not IHasInitializeAsync hasInitializeAsync) throw new Exception($"Command type {commandType.Name} does not implement IHasInitializeAsync.");
            if (!_commands.TryAdd(commandData.Name, (commandData, hasInitializeAsync))) throw new Exception($"Command name {commandType.Name} already exists under another class.");
        }
        return new CliArgsParser();
    }
}
