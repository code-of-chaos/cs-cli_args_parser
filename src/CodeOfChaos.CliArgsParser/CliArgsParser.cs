﻿// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CliArgsParser;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Immutable;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.RegularExpressions;

namespace CodeOfChaos.CliArgsParser;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <inheritdoc cref="ICliArgsParser"/>
public class CliArgsParser(IServiceProvider provider, CliArgsParserConfig configuration) : ICliArgsParser {
    /// <inheritdoc cref="ICliArgsParser.Config"/>
    public CliArgsParserConfig Config { get; } = configuration;

    /// <inheritdoc cref="ICliArgsParser.ParameterParsers"/>
    public ImmutableDictionary<Type, ICommandParameterParser> ParameterParsers => _parameterParsers ??= GetParameterParsersMap(provider, Config);
    private ImmutableDictionary<Type, ICommandParameterParser>? _parameterParsers;

    /// <inheritdoc cref="ICliArgsParser.Commands"/>
    public ImmutableDictionary<string, CommandMethodInfo> Commands => _commands ??= GetCommandsMap(provider, Config);
    private ImmutableDictionary<string, CommandMethodInfo>? _commands;

    // -----------------------------------------------------------------------------------------------------------------
    // Private Methods
    // -----------------------------------------------------------------------------------------------------------------
    #region Constructor Logic
    /// <summary>
    /// Gets the map of parameter parsers.
    /// </summary>
    /// <param name="provider">The service provider.</param>
    /// <param name="configuration">The configuration of the CLI args parser.</param>
    /// <returns>An immutable dictionary mapping parameter types to their corresponding parsers.</returns>
    private static ImmutableDictionary<Type, ICommandParameterParser> GetParameterParsersMap(IServiceProvider provider, CliArgsParserConfig configuration) {
        return new Dictionary<Type, ICommandParameterParser>(
            configuration.CommandParameterTypes.Select(
                type => new KeyValuePair<Type, ICommandParameterParser>(type, new CommandParameterParser(type, provider))
            )
        ).ToImmutableDictionary();
    }

    /// <summary>
    /// Retrieves a map of commands available in the CliArgsParser.
    /// </summary>
    /// <param name="provider">The service provider used for dependency injection.</param>
    /// <param name="configuration">The configuration for the CliArgsParser.</param>
    /// <returns>An immutable dictionary containing the commands available in the CliArgsParser.</returns>
    private static ImmutableDictionary<string, CommandMethodInfo> GetCommandsMap(IServiceProvider provider, CliArgsParserConfig configuration) {
        Dictionary<string, int> duplicateNameCache = new();
        Dictionary<string, CommandMethodInfo> commands = new();

        foreach (Type commandAtlasType in configuration.CommandAtlasTypes) {
            IEnumerable<(string, CommandMethodInfo)> enumerable = commandAtlasType
                .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
                .Select(info => (info, Attribute: info.GetCustomAttribute<CommandAttribute>(), DescriptionAttribute: info.GetCustomAttribute<DescriptionAttribute>()))
                .Where(tuple => tuple.Attribute is not null)
                .Select(tuple => new CommandMethodInfo(
                    tuple.info,
                    tuple.Attribute!,// can be suppressed because we already filter in "where"
                    (ICommandAtlas)ActivatorUtilities.CreateInstance(provider, commandAtlasType),
                    tuple.DescriptionAttribute
                ))
                .SelectMany(cmdInfo => {
                    var list = new List<(string, CommandMethodInfo)>();

                    string? tempName = null;
                    if (cmdInfo.CommandAttribute.Name is {} name) {
                        list.Add((name, cmdInfo));
                        tempName = name;
                    }
                    else if (RegexLib.SplitCamelCase.Matches(cmdInfo.Info.Name) is {} matches) {
                        List<Capture> groups = matches.SelectMany(match => match.Captures).ToList();
                        IEnumerable<Capture> sections = string.Equals(groups[0].Value, "command", StringComparison.InvariantCultureIgnoreCase)
                            ? groups.Skip(1)
                            : groups;
                        tempName = string.Join("-", sections.Select(s => s.Value.ToLowerInvariant()).Where(s => !string.IsNullOrEmpty(s) || !string.IsNullOrWhiteSpace(s)));
                        list.Add((tempName, cmdInfo));
                    }

                    if (!configuration.GenerateShortNames || tempName is null) return list;

                    name = string.Join(string.Empty, tempName.Split("-").Select(s => s.Trim()[0]));
                    list.Add((name, cmdInfo));

                    return list;
                });

            // Assemble the command dictionary and append numbers to duplicates
            foreach ((string name, CommandMethodInfo info) in enumerable) {
                string suffix = string.Empty;
                if (duplicateNameCache.TryGetValue(name, out int count)) {
                    suffix = count.ToString();
                    duplicateNameCache[name] = count + 1;
                }
                else {
                    duplicateNameCache[name] = 1;
                }

                if (commands.TryAdd($"{name}{suffix}", info)) continue;
                Console.WriteLine($"name duplicate {name}");
            }
        }

        return commands.ToImmutableDictionary();
    }
    #endregion
    #region Command Extraction Logic
    /// <summary>
    /// Tries to get the command name and arguments from the given input string.
    /// </summary>
    /// <param name="input">The input string containing the command name and arguments.</param>
    /// <param name="commandName">When this method returns, contains the command name if found; otherwise, null.</param>
    /// <param name="args">When this method returns, contains the arguments if found; otherwise, an empty dictionary.</param>
    /// <returns>True if the command name is found; otherwise, false.</returns>
    private static bool TryGetCommand(string input, [NotNullWhen(true)] out string? commandName, out Dictionary<string, string> args) {
        commandName = null;
        args = new Dictionary<string, string>();

        string[] strings = input.Split(" ", 2);

        if (strings.Length < 1) return false;

        commandName = strings[0];
        if (strings.Length == 2) args = GetArgs(strings[1]);

        return true;
    }

    /// <summary>
    /// Retrieves the arguments from the given input string and returns them as a dictionary.
    /// </summary>
    /// <param name="argsInput">The input string containing the arguments.</param>
    /// <returns>A dictionary containing the arguments.</returns>
    private static Dictionary<string, string> GetArgs(string argsInput) {
        return RegexLib.Args
            .Matches(argsInput)
            .Where(match => match.Success)
            .ToDictionary(
                keySelector: match => match.Groups[1].Value,
                elementSelector: match => match.Groups[2].Success
                    ? match.Groups[2].Value.Trim('"')
                    : "True"
            );
    }
    /// <summary>
    /// Represents a method that provides the information and parameters of a command.
    /// </summary>
    /// <param name="commandString">The string representing the command to execute.</param>
    /// <returns>A tuple containing the command information and its parameters.</returns>
    private (CommandMethodInfo, ICommandParameters) CommandMethodInfo(string commandString) {
        if (!TryGetCommand(commandString, out string? commandName, out Dictionary<string, string> args))
            throw new ArgumentException("Invalid command structure");

        if (!Commands.TryGetValue(commandName, out CommandMethodInfo commandMethodInfo))
            throw new ArgumentException("Invalid command name");

        if (!ParameterParsers.TryGetValue(commandMethodInfo.ParameterType, out ICommandParameterParser? parser)
            || !parser.TryParse(args, out ICommandParameters? parameters)
        ) throw new ArgumentException("Invalid command parameter type");
        return (commandMethodInfo, parameters);
    }
    #endregion
    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    /// <inheritdoc cref="ICliArgsParser.Execute"/>
    public void Execute(string commandString) {
        (CommandMethodInfo commandMethodInfo, ICommandParameters? parameters) = CommandMethodInfo(commandString);

        if (parameters.GetType() != typeof(NoArgs)) {
            commandMethodInfo.Delegate.DynamicInvoke(parameters);
        }
        else {
            commandMethodInfo.Delegate.DynamicInvoke();
        }
    }

    /// <inheritdoc cref="ICliArgsParser.ExecuteAsync"/>
    public async Task ExecuteAsync(string commandString) {
        (CommandMethodInfo commandMethodInfo, ICommandParameters? parameters) = CommandMethodInfo(commandString);

        switch (commandMethodInfo.Delegate) {
            case Func<Task> func when parameters is null or NoArgs:
                await func();// For Async methods without parameters
                return;

            case Action action when parameters is null or NoArgs:
                action();// For non-async methods without parameters
                return;

            case {} del when commandMethodInfo.IsAsync:
                if (parameters == null) {
                    throw new ArgumentException("No parameters provided for async method that needs parameters");
                }

                var task = (Task)del.DynamicInvoke(parameters)!;
                await task;
                return;

            case {} del:
                if (parameters == null) {
                    throw new ArgumentException("No parameters provided for method that needs parameters");
                }

                del.DynamicInvoke(parameters);// For non-async methods with parameters
                return;

            default:
                throw new ConstraintException("Delegate could not be invoked");
        }
    }
}
