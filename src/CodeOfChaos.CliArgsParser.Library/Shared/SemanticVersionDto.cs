// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.CliArgsParser.Library.Commands.VersionBump;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace CodeOfChaos.CliArgsParser.Library.Shared;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public partial class SemanticVersionDto {
    private uint Major { get; set; }
    private uint Minor { get; set; }
    private uint Patch { get; set; }
    private uint? Preview { get; set; }

    [GeneratedRegex(@"^(?<major>\d+)\.(?<minor>\d+)\.(?<patch>\d+)(-preview\.(?<preview>\d+))?$")]
    private static partial Regex FindVersionRegex { get; }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public static bool TryParse(string version, [NotNullWhen(true)] out SemanticVersionDto? result) {
        Match match = FindVersionRegex.Match(version);
        if (!match.Success) {
            result = null;
            return false;
        }

        result = new SemanticVersionDto {
            Major = uint.Parse(match.Groups["major"].Value),
            Minor = uint.Parse(match.Groups["minor"].Value),
            Patch = uint.Parse(match.Groups["patch"].Value),
            Preview = match.Groups["preview"].Success ? uint.Parse(match.Groups["preview"].Value) : null
        };

        return true;
    }

    public void BumpVersion(VersionSection section) {
        switch (section) {
            case VersionSection.Major: {
                Major += 1;
                Minor = 0;
                Patch = 0;
                Preview = null;
                break;
            }

            case VersionSection.Minor: {
                Minor += 1;
                Patch = 0;
                Preview = null;
                break;
            }

            case VersionSection.Patch: {
                Patch += 1;
                Preview = null;
                break;
            }

            case VersionSection.Preview: {
                Preview ??= 0;
                Preview += 1;
                break;
            }

            case VersionSection.Manual: {
                // Ask the user for input
                int tries = 0;
                while (tries <= 5) {
                    Console.WriteLine("Please enter the new version");
                    string? input = Console.ReadLine();
                    if (input is null || !TryParse(input, out SemanticVersionDto? newVersion)) {
                        Console.WriteLine("Invalid input");
                        tries++;
                        continue;
                    }
                    Major = newVersion.Major;
                    Minor = newVersion.Minor;
                    Patch = newVersion.Patch;
                    Preview = newVersion.Preview;
                    break;
                }
                throw new Exception("Could not parse version after 5 tries.");
            }

            // We don't care
            case VersionSection.None:
            default:
                break;
        }
    }

    public override string ToString() => Preview is not null
        ? $"{Major}.{Minor}.{Patch}-preview.{Preview}"
        : $"{Major}.{Minor}.{Patch}";
}
