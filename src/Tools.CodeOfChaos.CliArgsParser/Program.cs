// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.CliArgsParser;
using CodeOfChaos.CliArgsParser.Library;

namespace Tools.CodeOfChaos.CliArgsParser;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class Program {
    public static async Task Main(string[] args) {
        var parser = CliArgsBuilder.CreateFromConfig(
            config => {
                config.AddCommandsFromAssemblyEntrypoint<IAssemblyEntry>();
            }
        ).Build();

        await parser.ParseAsync(args);
    }
}
