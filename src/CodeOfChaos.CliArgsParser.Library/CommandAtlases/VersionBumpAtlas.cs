// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using AterraEngine.Unions;
using CliArgsParser;
using System.Diagnostics;
using System.Xml;
using System.Xml.Linq;
using static CodeOfChaos.Ansi.AnsiColor;


namespace CodeOfChaos.CliArgsParser.Library.CommandAtlases;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
// ReSharper disable once UnusedType.Global
public class VersionBumpAtlas : ICommandAtlas {
    [Command<VersionBumpParameters>("bump")]
    public async Task VersionBumpCommand(VersionBumpParameters args) {
        Console.WriteLine(Fore("orange", "Starting execution 'VersionBump' "));
        
        Console.WriteLine(Fore("white","Bumping version..."));
        SuccessOrFailure<string> bumpResult = await BumpVersion(args);
        if (bumpResult is { IsFailure: true, AsFailure.Value: var errorBumping }) {
            Console.WriteLine(errorBumping);
            return;
        }
        
        string updatedVersion = bumpResult.AsSuccess.Value;
        
        Console.WriteLine(Fore("white","Git commiting..."));
        SuccessOrFailure gitCommitResult = await TryCreateGitCommit(updatedVersion);
        if (gitCommitResult is { IsFailure: true, AsFailure.Value: var errorCommiting }) {
            Console.WriteLine(Fore("red", errorCommiting));
            return;
        }
        
        Console.WriteLine(Fore("white","Git tagging..."));
        SuccessOrFailure gitTagResult = await TryCreateGitTag(updatedVersion);
        if (gitTagResult is { IsFailure: true, AsFailure.Value: var errorTagging }) {
            Console.WriteLine(Fore("red", errorTagging));
            return;
        }
        
        Console.WriteLine($"Version {updatedVersion} committed and tagged successfully.");
    }
    
    private static async Task<SuccessOrFailure> TryCreateGitTag(string updatedVersion) {
        var gitTagInfo = new ProcessStartInfo("git", "tag v" + updatedVersion) {
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using Process? gitTagProcess = Process.Start(gitTagInfo);
        Console.WriteLine(await gitTagProcess?.StandardOutput.ReadToEndAsync()!);
        await gitTagProcess.WaitForExitAsync();
        
        if (gitTagProcess.ExitCode != 0) return "Git Tagging failed";
        return new Success();

    }
    private static async Task<SuccessOrFailure> TryCreateGitCommit(string updatedVersion) {

        var gitCommitInfo = new ProcessStartInfo("git", $"commit -am \"VersionBump : v{updatedVersion}\"") {
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using Process? gitCommitProcess = Process.Start(gitCommitInfo);
        Console.WriteLine(await gitCommitProcess?.StandardOutput.ReadToEndAsync()!);
        await gitCommitProcess.WaitForExitAsync();
        
        if (gitCommitProcess.ExitCode != 0) return "Git Commit failed";
        return new Success();
    }


    private static async Task<SuccessOrFailure<string>> BumpVersion(VersionBumpParameters args) {
        string[] projectFiles = args.Projects;
        if (projectFiles.Length == 0) {
            return new Failure<string>("No projects specified");
        }
        
        VersionSection sectionToBump = args.Section;
        string? versionToReturn = null;

        foreach (string projectFile in projectFiles) {
            string path = Path.Combine(args.Root, args.SourceFolder, projectFile, projectFile + ".csproj");
            if (!File.Exists(path)) {
                return new Failure<string>($"Could not find project file {projectFile}");
            }

            XDocument document;
            await using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true)) {
                document = await XDocument.LoadAsync(stream, LoadOptions.PreserveWhitespace, CancellationToken.None);
            }

            XElement? versionElement = document
                .Descendants("PropertyGroup")
                .Elements("Version")
                .FirstOrDefault();

            if (versionElement == null) {
                return new Failure<string>($"File {projectFile} did not contain a version element");
            }

            string[] versionParts = versionElement.Value.Split('.');
            if (versionParts.Length != 3) {
                return new Failure<string>($"File {projectFile} contained an invalid version element: {versionElement.Value}");
            }

            switch (sectionToBump) {
                case VersionSection.Major:
                    versionParts[0] = (int.Parse(versionParts[0]) + 1).ToString();
                    versionParts[1] = "0";
                    versionParts[2] = "0";
                    break;
                case VersionSection.Minor:
                    versionParts[1] = (int.Parse(versionParts[1]) + 1).ToString();
                    versionParts[2] = "0";
                    break;
                case VersionSection.Patch:
                    versionParts[2] = (int.Parse(versionParts[2]) + 1).ToString();
                    break;

                case VersionSection.None:
                default: {
                    return new Failure<string>($"Invalid version section {sectionToBump}");
                }
            }

            versionElement.Value = versionToReturn ??= string.Join(".", versionParts);

            var settings = new XmlWriterSettings {
                Indent = true,
                IndentChars = "    ", 
                Async = true,
                OmitXmlDeclaration = true,
            };
            await using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true)) {
                await using var writer = XmlWriter.Create(stream, settings);
                document.Save(writer);
            }
            
            Console.WriteLine($"Updated {projectFile} version to {versionElement.Value}");
        }

        return versionToReturn is not null
            ? new Success<string>(versionToReturn)
            : new Failure<string>("Could not find a version to bump");
    }
}
