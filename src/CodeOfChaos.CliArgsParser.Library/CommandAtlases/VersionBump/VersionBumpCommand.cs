// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using AterraEngine.Unions;
using CodeOfChaos.CliArgsParser.Attributes;
using CodeOfChaos.CliArgsParser.Contracts;
using CodeOfChaos.CliArgsParser.Library.Shared;
using System.Xml.Linq;

namespace CodeOfChaos.CliArgsParser.Library.CommandAtlases.VersionBump;
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
        Console.WriteLine("Bumping version...");
        SuccessOrFailure<SemanticVersionDto> bumpResult = await BumpVersion(parameters);
        if (bumpResult is { IsFailure: true, AsFailure.Value: var errorBumping }) {
            Console.WriteLine(errorBumping);
            return;
        }

        SemanticVersionDto updatedVersion = bumpResult.AsSuccess.Value;

        Console.WriteLine("Git committing ...");
        SuccessOrFailure gitCommitResult = await GitHelpers.TryCreateGitCommit(updatedVersion);
        if (gitCommitResult is { IsFailure: true, AsFailure.Value: var errorCommiting }) {
            Console.WriteLine(errorCommiting);
            return;
        }

        Console.WriteLine("Git tagging ...");
        SuccessOrFailure gitTagResult = await GitHelpers.TryCreateGitTag(updatedVersion);
        if (gitTagResult is { IsFailure: true, AsFailure.Value: var errorTagging }) {
            Console.WriteLine(errorTagging);
            return;
        }

        Console.WriteLine($"Version {updatedVersion} committed and tagged successfully.");

        if (!parameters.PushToRemote) return;

        Console.WriteLine("Pushing to origin ...");
        SuccessOrFailure pushResult = await GitHelpers.TryPushToOrigin();
        if (pushResult is { IsFailure: true, AsFailure.Value: var errorPushing }) {
            Console.WriteLine(errorPushing);
            return;
        }

        Console.WriteLine("Pushed to origin successfully.");
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

            if (versionElement == null) {
                return new Failure<string>($"File did not contain a version element");
            }

            if (versionDto is null) {
                if (!SemanticVersionDto.TryParse(versionElement.Value, out SemanticVersionDto? dto)) {
                    return new Failure<string>($"File contained an invalid version element: {versionElement.Value}");
                }

                dto.BumpVersion(sectionToBump);

                versionDto = dto;
            }

            versionElement.Value = versionDto.ToString();
            Console.WriteLine($"Updated version to {versionElement.Value}");
        }

        return versionDto is not null
            ? new Success<SemanticVersionDto>(versionDto)
            : new Failure<string>("Could not find a version to bump");
    }
}
