// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using CodeOfChaos.CliArgsParser.Attributes;
using CodeOfChaos.CliArgsParser.Contracts;

namespace CodeOfChaos.CliArgsParser.Library.CommandAtlases.DownloadIcon;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[CliArgsCommand("download-icon")]
[CliArgsDescription("Downloads the icon for the specified project.")]
public partial class DownloadIconCommand : ICommand<DownloadIconParameters> {

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public Task ExecuteAsync(DownloadIconParameters parameters) {
        throw new NotImplementedException();
    }
}
