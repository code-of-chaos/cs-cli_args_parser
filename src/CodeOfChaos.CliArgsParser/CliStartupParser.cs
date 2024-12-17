// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.CliArgsParser.Contracts;

namespace CodeOfChaos.CliArgsParser;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class StartupArgsParser
{
    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public static TParameter Parse<TParameter>(string[] args) 
        where TParameter : struct, IParameters 
    {
        string input = InputHelper.ToOneLine(args);
        
        var registry = new UserInputRegistry();
        registry.IngestString(input);
        
        var parameter = new TParameter().NewFromRegistry<TParameter>(registry);
        return parameter;
    }

    public static bool TryParse<TParameter>(string[] args, out TParameter parameter)
        where TParameter : struct, IParameters
    {
        parameter = default;
        try {
            parameter = Parse<TParameter>(args);
            return true;
        }
        catch (Exception) {
            return false;
        }
    }

    public static Task ParseAndExecuteAsync<TCommand, TParameter>(string[] args)
        where TCommand : class, ICommand<TParameter>
        where TParameter : struct, IParameters 
    {
        string input = InputHelper.ToOneLine(args);
        
        var registry = new UserInputRegistry();
        registry.IngestString(input);
        
        var command = Activator.CreateInstance<TCommand>();
        command.InitializeAsync(registry);
        return Task.CompletedTask;
    }

    public static async Task<bool> TryParseAndExecuteAsync<TCommand, TParameter>(string[] args)
        where TCommand : class, ICommand<TParameter> 
        where TParameter : struct, IParameters
    {
        try {
            await ParseAndExecuteAsync<TCommand, TParameter>(args);
            return true;
        }
        catch (Exception ex) {
            return false;
        }
    }
}
