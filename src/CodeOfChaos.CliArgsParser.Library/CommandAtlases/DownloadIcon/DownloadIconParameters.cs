// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.CliArgsParser.Attributes;
using CodeOfChaos.CliArgsParser.Contracts;

namespace CodeOfChaos.CliArgsParser.Library.CommandAtlases.DownloadIcon;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public readonly partial struct DownloadIconParameters : IParameters {
    [CliArgsParameter("root", "r")] [CliArgsDescription("The root directory of the project to update")]
    public string Root { get; init; } = "../../../../../";

    [CliArgsParameter("push", "p", ParameterType.Flag)] [CliArgsDescription("Push the changes to the remote repository")]
    public bool PushToRemote { get; init; } = false;

    [CliArgsParameter("projects", "pr")] [CliArgsDescription("The projects to update")]
    public required string ProjectsStringValue { get; init; }

    [CliArgsParameter("project-split", "ps")] [CliArgsDescription("The split character to use when splitting the projects string")]
    public string ProjectsSplit { get; init; } = ";";

    [CliArgsParameter("source-folder", "sf")] [CliArgsDescription("The folder where the projects are located")]
    public string SourceFolder { get; init; } = "src";

    [CliArgsParameter("icon-folder", "if")] [CliArgsDescription("The folder where the icons are located")]
    public string IconFolder { get; init; } = "assets/";

    [CliArgsParameter("origin", "o")] [CliArgsDescription("The origin of the icon file")]
    public required string Origin { get; init; }

    public string[] GetProjects() => ProjectsStringValue
        .Split(ProjectsSplit)
        .Where(entry => !string.IsNullOrWhiteSpace(entry))
        .ToArray();

}
