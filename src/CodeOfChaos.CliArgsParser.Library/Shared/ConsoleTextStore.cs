// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.Ansi;

namespace CodeOfChaos.CliArgsParser.Library.Shared;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class ConsoleTextStore {
    private static AnsiStringBuilder Builder { get; } = new();

    public static string BumpingVersion => Builder.Fore
        .AppendWhitesmoke("Defining and Bumping version ")
        .AppendSlategray("...")
        .ToStringAndClear();
    
    public static string GitCommitting => Builder.Fore
        .AppendWhitesmoke("Git committing ")
        .AppendSlategray("...")
        .ToStringAndClear();
    
    public static string GitTagging => Builder.Fore
        .AppendWhitesmoke("Git tagging ")
        .AppendSlategray("...")
        .ToStringAndClear();
    
    public static string GitPushingToRemote => Builder.Fore
        .AppendWhitesmoke("Pushing to origin ")
        .AppendSlategray("...")
        .ToStringAndClear();
    
    public static string QuestionTagAndCommit => Builder
        .WithFore(f => f
            .AppendWhitesmoke("Do you want to Git tag & push to origin?")
            .AppendSlategray(" (y/n)")
        ).ToStringAndClear();
    
    public static string CommandEndSuccess() => Builder.Fore
        .AppendGreen("Command completed successfully.")
        .ToStringAndClear();
    
    public static string CommandEndFailure(string? failure) => Builder.Fore
        .WithFore(f => {
            f.AppendCrimson("Command failed");
            if (failure is not null) f.AppendCrimson(" : ").AppendWhitesmoke(failure);
        }).ToStringAndClear();

    public static string TagSuccessful(SemanticVersionDto versionDto) => Builder
        .WithFore(f => f
            .AppendWhitesmoke("Version ")
            .AppendDeepskyblue(versionDto.ToString())
            .AppendWhitesmoke(" successfully git tagged and committed.")
        ).ToStringAndClear();
    
    public static string UpdatedVersion(string projectName, string versionElement) => Builder
        .WithFore(f => f
            .AppendWhitesmoke("Updated version of package ")
            .AppendDeepskyblue(projectName)
            .AppendWhitesmoke(" to ")
            .AppendDeepskyblue(versionElement)
        ).ToStringAndClear();
}
