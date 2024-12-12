// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CliArgsParser;

namespace CodeOfChaos.CliArgsParser.Library.CommandAtlases;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class DownloadIconParameters : ICommandParameters {
    [ArgValue("root", "r")] [Description("The root directory of the solution")]
    public string Root { get; set; } = "../../../../../";
    
    [ArgValue("destination", "d")] [Description("The destination folder to download the icons to")]
    public string Destination { get; set; } = "assets/icon.png";
    
    [ArgValue("origin", "o")] [Description("The icon to download")]
    public string? Origin { get; set; }
    
    [ArgValue("projects", "p")] [Description("The projects to update")]
    public string? ProjectsStringValue { get; set; }
    
    [ArgValue("project-split", "ps")] [Description("The split character to use when splitting the projects string")]
    public string ProjectsSplit { get; set; } = ";";
    
    private string[]? _projects;
    public string[] Projects => _projects ??= ProjectsStringValue?
            .Split(ProjectsSplit)
            .Where(entry => !string.IsNullOrWhiteSpace(entry))
            .ToArray()
        ?? [];
}
