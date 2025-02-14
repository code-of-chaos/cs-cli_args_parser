// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using AterraEngine.Unions;
using CodeOfChaos.Ansi;
using System.Diagnostics;

namespace CodeOfChaos.CliArgsParser.Library.Shared;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class GitHelpers {
    public static async Task<SuccessOrFailure> TryPushToOrigin() {
        var gitTagInfo = new ProcessStartInfo("git", "push origin") {
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        var builder = new AnsiStringBuilder();

        using Process? gitTagProcess = Process.Start(gitTagInfo);

        builder.Fore.AppendWhitesmokeLine(await gitTagProcess?.StandardOutput.ReadToEndAsync()!);
        Console.WriteLine(builder.ToStringAndClear());
        
        await gitTagProcess.WaitForExitAsync();

        if (gitTagProcess.ExitCode != 0) return "Push to origin failed";

        return new Success();
    }
    
    
    public static async Task<SuccessOrFailure> TryPushTagsToOrigin() {
        var gitTagInfo = new ProcessStartInfo("git", "push origin --tags") {
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        var builder = new AnsiStringBuilder();

        using Process? gitTagProcess = Process.Start(gitTagInfo);

        builder.Fore.AppendWhitesmokeLine(await gitTagProcess?.StandardOutput.ReadToEndAsync()!);
        Console.WriteLine(builder.ToStringAndClear());
        
        await gitTagProcess.WaitForExitAsync();

        if (gitTagProcess.ExitCode != 0) return "Pushing Tags to origin failed";

        return new Success();
    }
    

    public static async Task<SuccessOrFailure> TryCreateGitTag(SemanticVersionDto updatedVersion) {
        var gitTagInfo = new ProcessStartInfo("git", "tag v" + updatedVersion) {
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        var builder = new AnsiStringBuilder();

        using Process? gitTagProcess = Process.Start(gitTagInfo);

        builder.Fore.AppendWhitesmokeLine(await gitTagProcess?.StandardOutput.ReadToEndAsync()!);
        Console.WriteLine(builder.ToStringAndClear());
        
        await gitTagProcess.WaitForExitAsync();

        if (gitTagProcess.ExitCode != 0) return "Git Tagging failed";

        return new Success();
    }

    public static async Task<SuccessOrFailure> TryCreateGitCommit(SemanticVersionDto updatedVersion) {
        var gitCommitInfo = new ProcessStartInfo("git", $"commit -am \"VersionBump : v{updatedVersion}\"") {
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        var builder = new AnsiStringBuilder();

        using Process? gitCommitProcess = Process.Start(gitCommitInfo);

        builder.Fore.AppendWhitesmokeLine(await gitCommitProcess?.StandardOutput.ReadToEndAsync()!);
        Console.WriteLine(builder.ToStringAndClear());
        
        await gitCommitProcess.WaitForExitAsync();

        if (gitCommitProcess.ExitCode != 0) return "Git Commit failed";

        return new Success();
    }
}
