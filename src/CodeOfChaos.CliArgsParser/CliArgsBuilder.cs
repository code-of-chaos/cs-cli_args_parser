// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.CliArgsParser.Contracts;

namespace CodeOfChaos.CliArgsParser;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class CliArgsBuilder(CliArgsBuilderConfig config) {
    private readonly Dictionary<string, CommandData> _commands = new();
    
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
            var command = (IHasCommandData) Activator.CreateInstance(commandType)!;
            
            if (!_commands.TryAdd(command.CommandData.Name, command.CommandData)) {
                throw new Exception($"Command name {commandType.Name} already exists under another class.");
            }
        }
        return new CliArgsParser();
    }
}