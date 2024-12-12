// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CliArgsParser;
using static CodeOfChaos.Ansi.AnsiColor;

namespace CodeOfChaos.CliArgsParser.Library.CommandAtlases;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class DownloadIconAtlas : ICommandAtlas {
    [Command<DownloadIconParameters>("download-icon")]
    public async Task DownloadIcon(DownloadIconParameters parameters) {
        // Todo use new CodeOfChaos.Ansi markup
        Console.WriteLine(Fore("orange", "Starting execution 'VersionBump' "));
        
        // Todo create a downloader for the icon.png assets uses for nuget packages
    }
    
}
