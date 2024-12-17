// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Threading.Tasks;

namespace CodeOfChaos.CliArgsParser.Generators.Sample;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class Program {
    public static async Task Main(string[] args) {
        CliArgsParser parser = CliArgsBuilder.CreateFromConfig(
            config => {
                config.AddCommand<ExampleCommand>();
            }
        ).Build();

        await parser.ParseAsync(args);
    }
}
