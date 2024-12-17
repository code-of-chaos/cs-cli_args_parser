// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.CliArgsParser.Contracts;
using System.Collections.Frozen;

namespace CodeOfChaos.CliArgsParser;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class CliArgsBuilder(CliArgsBuilderConfig config) {
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
        Dictionary<string, (CommandData, INonGenericCommandInterfaces)> commands = new();
        
        while (config.Commands.TryPop(out Type? commandType)) {
            (CommandData CommandData, INonGenericCommandInterfaces) tuple = ConvertTypeToCommand(commandType);
            if (!commands.TryAdd(tuple.CommandData.Name, tuple)) throw new Exception($"Command name {commandType.Name} already exists under another class.");
        }
        
        return new CliArgsParser {
            CommandLookup = commands.ToFrozenDictionary(),
            StartupCommand = config.StartupCommand is not null 
                ? ConvertTypeToCommand(config.StartupCommand)
                : null
        };
    }

    private static (CommandData, INonGenericCommandInterfaces) ConvertTypeToCommand(Type type) {
        if (Activator.CreateInstance(type) is not INonGenericCommandInterfaces { CommandData: var commandData } command) 
            throw new Exception($"Command type {type.Name} does not implement INonGenericCommandInterfaces.");
        return (commandData, command);
    }
}
