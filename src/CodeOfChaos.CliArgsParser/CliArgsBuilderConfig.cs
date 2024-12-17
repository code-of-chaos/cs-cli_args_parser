// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.CliArgsParser.Attributes;
using CodeOfChaos.CliArgsParser.Contracts;
using System.Reflection;

namespace CodeOfChaos.CliArgsParser;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class CliArgsBuilderConfig {
    internal Stack<Type> Commands { get; } = new();
    internal Type? StartupCommand { get; set; }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public CliArgsBuilderConfig AddCommand<T>() => AddCommand(typeof(T));

    public CliArgsBuilderConfig AddCommand(Type commandType) {
        Commands.Push(commandType);
        return this;
    }

    public CliArgsBuilderConfig AddStartupCommand<T>() => AddStartupCommand(typeof(T));

    public CliArgsBuilderConfig AddStartupCommand(Type commandType) {
        if (StartupCommand is not null) throw new Exception("Startup command already set.");

        StartupCommand = commandType;
        return this;
    }

    public void AddCommandsFromAssemblyEntrypoint<T>() => AddCommandsFromAssemblyEntrypoint(typeof(T));
    public void AddCommandsFromAssemblyEntrypoint(Type entrypoint) => AddCommandsFromAssembly(entrypoint.Assembly);

    public void AddCommandsFromAssembly(Assembly assembly) {
        // Get all types in the provided assembly
        IEnumerable<Type> commandTypes = assembly.GetTypes()
            .Where(type =>
                type.IsClass// Must be a class
                && !type.IsAbstract// Must not be abstract
                && type.IsDefined(typeof(CliArgsCommandAttribute), false)// Must have CliArgsCommandAttribute
                && type.GetInterfaces().Any(i =>
                        i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommand<>)// Must implement ICommand<T>
                )
            );

        // Add each command type to the configuration
        foreach (Type commandType in commandTypes) {
            AddCommand(commandType);
        }
    }
}
