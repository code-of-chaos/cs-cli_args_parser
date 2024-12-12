// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CliArgsParser;

namespace CodeOfChaos.CliArgsParser.Library.CommandAtlases;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class VersionBumpParameters : ICommandParameters {
    [ArgValue("root")] [Description("The root directory of the project to update")]
    public string Root { get; set; } = "../../../../../";
    
    [ArgValue("section")] [Description("The section of the version to bump. One of: Major, Minor, Patch")]
    public string? SectionStringValue { get; set; }
    
    [ArgValue("projects")] [Description("The projects to update")]
    public string? ProjectsStringValue { get; set; }
    
    [ArgValue("project-split")] [Description("The split character to use when splitting the projects string")]
    public string ProjectsSplit { get; set; } = ";";
    
    [ArgValue("source-folder")] [Description("The folder where the projects are located")]
    public string SourceFolder { get; set; } = "src";
    
    private string[]? _projects;
    public string[] Projects => _projects ??= ProjectsStringValue?
            .Split(ProjectsSplit)
            .Where(entry => !string.IsNullOrWhiteSpace(entry))
            .ToArray()
        ?? [];


    public VersionSection Section => Enum.Parse<VersionSection>(SectionStringValue ?? "None", ignoreCase:true);
}

public enum VersionSection {
    Major,
    Minor,
    Patch,
    None
}
