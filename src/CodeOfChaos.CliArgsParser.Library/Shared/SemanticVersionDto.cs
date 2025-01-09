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
                return;
            }

            case VersionSection.Minor: {
                Minor += 1;
                Patch = 0;
                Preview = null;
                return;
            }

            case VersionSection.Patch: {
                Patch += 1;
                Preview = null;
                return;
            }

            case VersionSection.Preview when Preview is null: {
                SemanticVersionDto newVersion = FromInput("Please enter a semantic version for the new preview version:");
                
                Major = newVersion.Major;
                Minor = newVersion.Minor;
                Patch = newVersion.Patch;
                Preview = newVersion.Preview ?? 0 ;
                return;
            }

            case VersionSection.Preview: {
                Preview += 1;
                return;
            }

            case VersionSection.Manual: {
                SemanticVersionDto newVersion = FromInput();
                
                Major = newVersion.Major;
                Minor = newVersion.Minor;
                Patch = newVersion.Patch;
                Preview = newVersion.Preview;
                return;
            }

            // We don't care
            case VersionSection.None:
            default:
                return;
        }
    }

    public override string ToString() => Preview is not null
        ? $"{Major}.{Minor}.{Patch}-preview.{Preview}"
        : $"{Major}.{Minor}.{Patch}";

    private static SemanticVersionDto FromInput(string inputText = "Please enter a semantic version:") {
        int tries = 0;
        while (tries <= 5) {
            Console.WriteLine(inputText);
            string? input = Console.ReadLine();
            if (input is not null && TryParse(input, out SemanticVersionDto? newVersion)) return newVersion;

            Console.WriteLine("Invalid input");
            tries++;
        }

        throw new Exception("Could not parse version after 5 tries.");
    }
}
