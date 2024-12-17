// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.CliArgsParser.Attributes;
using CodeOfChaos.CliArgsParser.Contracts;

namespace CodeOfChaos.CliArgsParser.Library.CommandAtlases.VersionBump;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public readonly partial struct VersionBumpParameters : IParameters {
    [CliArgsParameter("root", "r"), CliArgsDescription("The root directory of the project to update")]
    public string Root { get; init; } = "../../../../../";
    
    [CliArgsParameter("section", "s"), CliArgsDescription("The section of the version to bump. One of: Major, Minor, Patch")]
    public required string? SectionStringValue { get; init; }
    public VersionSection Section => Enum.Parse<VersionSection>(SectionStringValue ?? "None", ignoreCase:true);
    
    [CliArgsParameter("push", "p", ParameterType.Flag), CliArgsDescription("Push the changes to the remote repository")]
    public bool PushToRemote { get; init; } = false;
    
    [CliArgsParameter("projects", "pr"), CliArgsDescription("The projects to update")]
    public required string ProjectsStringValue { get; init; }
    
    [CliArgsParameter("project-split", "ps"), CliArgsDescription("The split character to use when splitting the projects string")]
    public string ProjectsSplit { get; init; } = ";";
    
    [CliArgsParameter("source-folder", "sf"), CliArgsDescription("The folder where the projects are located")]
    public string SourceFolder { get; init; } = "src";

    public string[] GetProjects() => ProjectsStringValue
        .Split(ProjectsSplit)
        .Where(entry => !string.IsNullOrWhiteSpace(entry))
        .ToArray();

}
