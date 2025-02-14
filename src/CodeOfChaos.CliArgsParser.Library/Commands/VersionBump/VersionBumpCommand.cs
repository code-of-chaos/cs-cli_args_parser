// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using AterraEngine.Unions;
using CodeOfChaos.CliArgsParser.Library.Shared;
using System.Xml.Linq;

namespace CodeOfChaos.CliArgsParser.Library.Commands.VersionBump;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[CliArgsCommand("git-version-bump")]
[CliArgsDescription("Bumps the version of the projects specified in the projects argument.")]
public partial class VersionBumpCommand : ICommand<VersionBumpParameters> {

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public async Task ExecuteAsync(VersionBumpParameters parameters) {
        Console.WriteLine(ConsoleTextStore.BumpingVersion);
        SuccessOrFailure<SemanticVersionDto> bumpResult = await BumpVersion(parameters);
        if (bumpResult is { IsFailure: true, AsFailure.Value: var errorBumping }) {
            Console.WriteLine(ConsoleTextStore.CommandEndFailure(errorBumping));
            return;
        }

        SemanticVersionDto updatedVersion = bumpResult.AsSuccess.Value;
        
        // Ask the user for extra input to make sure they want to commit and the current tag.
        if (!parameters.Force) {
            Console.WriteLine(ConsoleTextStore.QuestionTagAndCommit);
            char? input = Console.ReadLine()?.ToLowerInvariant().FirstOrDefault();
            if (input is not 'y') {
                Console.WriteLine(ConsoleTextStore.CommandEndSuccess());
                return;
            }
        }

        Console.WriteLine(ConsoleTextStore.GitCommitting);
        SuccessOrFailure gitCommitResult = await GitHelpers.TryCreateGitCommit(updatedVersion);
        if (gitCommitResult is { IsFailure: true, AsFailure.Value: var errorCommiting }) {
            Console.WriteLine(ConsoleTextStore.CommandEndFailure(errorCommiting));
            return;
        }

        Console.WriteLine(ConsoleTextStore.GitTagging);
        SuccessOrFailure gitTagResult = await GitHelpers.TryCreateGitTag(updatedVersion);
        if (gitTagResult is { IsFailure: true, AsFailure.Value: var errorTagging }) {
            Console.WriteLine(ConsoleTextStore.CommandEndFailure(errorTagging));
            return;
        }

        Console.WriteLine(ConsoleTextStore.TagSuccessful(updatedVersion));

        if (!parameters.PushToRemote) {
            Console.WriteLine(ConsoleTextStore.CommandEndSuccess());
            return;
        }

        Console.WriteLine(ConsoleTextStore.GitPushingToRemote);
        SuccessOrFailure pushResult = await GitHelpers.TryPushToOrigin();
        if (pushResult is { IsFailure: true, AsFailure.Value: var errorPushing }) {
            Console.WriteLine(ConsoleTextStore.CommandEndFailure(errorPushing));
            return;
        }
        
        SuccessOrFailure pushTagsResult = await GitHelpers.TryPushTagsToOrigin();
        if (pushTagsResult is { IsFailure: true, AsFailure.Value: var errorPushingTags }) {
            Console.WriteLine(ConsoleTextStore.CommandEndFailure(errorPushingTags));
            return;
        }

        Console.WriteLine(ConsoleTextStore.CommandEndSuccess());
    }


    private static async Task<SuccessOrFailure<SemanticVersionDto>> BumpVersion(VersionBumpParameters args) {
        string[] projectFiles = CsProjHelpers.AsProjectPaths(args.Root, args.SourceFolder, args.GetProjects());
        if (projectFiles.Length == 0) {
            return new Failure<string>("No projects specified");
        }

        VersionSection sectionToBump = args.Section;
        SemanticVersionDto? versionDto = null;

        await foreach (XDocument document in CsProjHelpers.GetProjectFiles(projectFiles)) {
            XElement? versionElement = document
                .Descendants("PropertyGroup")
                .Elements("Version")
                .FirstOrDefault();

            // Only needed for logging, so setting to "UNKNOWN" is okay
            string projectName = document
                .Descendants("PropertyGroup")
                .Elements("PackageId")
                .FirstOrDefault()?
                .Value ?? "UNKNOWN";

            if (versionElement == null) {
                return new Failure<string>("File did not contain a version element");
            }

            if (versionDto is null) {
                if (!SemanticVersionDto.TryParse(versionElement.Value, out SemanticVersionDto? dto))
                    return new Failure<string>($"File contained an invalid version element: {versionElement.Value}");

                dto.BumpVersion(sectionToBump);

                versionDto = dto;
            }

            versionElement.Value = versionDto.ToString();
            Console.WriteLine(ConsoleTextStore.UpdatedVersion(projectName, versionElement.Value));
        }

        return versionDto is not null
            ? new Success<SemanticVersionDto>(versionDto)
            : new Failure<string>("Could not find a version to bump");
    }
}
