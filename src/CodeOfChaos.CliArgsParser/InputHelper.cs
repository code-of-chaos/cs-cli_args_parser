// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
namespace CodeOfChaos.CliArgsParser;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class InputHelper {
    public static string ToOneLine(string[] input) {
        return string.Join(" ",
            input.Select(arg => {
                // if it has already been corrected, just return as is
                if (arg.Contains("=\"") && arg.EndsWith('"')) return arg;

                // Wrap in quotes if it is a kvp
                if (arg.Contains('=')) {
                    string[] parts = arg.Split('=', 2);
                    return $"{parts[0]}=\"{parts[1]}\"";
                }

                // Wrap in quotes if it contains spaces and is not a flag or kvp \";
                if (arg.Contains(' ') && !arg.StartsWith('-')) return $"\"{arg}\"";

                // Leave other arguments unchanged
                return arg;
            })
        );
    }
}
