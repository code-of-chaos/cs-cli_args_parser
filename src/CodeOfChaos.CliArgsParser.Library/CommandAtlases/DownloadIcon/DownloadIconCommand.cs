// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using AterraEngine.Unions;
using CodeOfChaos.CliArgsParser.Library.Shared;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace CodeOfChaos.CliArgsParser.Library.CommandAtlases.DownloadIcon;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
[CliArgsCommand("nuget-download-icon")]
[CliArgsDescription("Downloads and assigns the icon, to be used as nuget package's icon, for the specified project.")]
public partial class DownloadIconCommand : ICommand<DownloadIconParameters> {

    [GeneratedRegex(@"^[^/\\\s]+$")]
    private static partial Regex IsEmptyFolderNameRegex { get; }

    // -----------------------------------------------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------------------------------------------
    public async Task ExecuteAsync(DownloadIconParameters parameters) {
        Console.WriteLine("Downloading Icon...");
        SuccessOrFailure getResult = await TryGetIcon(parameters);
        if (getResult is { IsFailure: true, AsFailure.Value: var getError }) {
            Console.WriteLine(getError);
        }

        Console.WriteLine("Icon downloaded successfully.");
        SuccessOrFailure applyResult = await ApplyIcon(parameters);
        if (applyResult is { IsFailure: true, AsFailure.Value: var applyError }) {
            Console.WriteLine(applyError);
        }
        Console.WriteLine("Icon applied successfully.");
    }

    private static async Task<SuccessOrFailure> ApplyIcon(DownloadIconParameters args) {
        string[] projectFiles = CsProjHelpers.AsProjectPaths(args.Root, args.SourceFolder, args.GetProjects());
        if (projectFiles.Length == 0) {
            return new Failure<string>("No projects specified");
        }

        await foreach (XDocument document in CsProjHelpers.GetProjectFiles(projectFiles)) {

            // Loop through each project file's XML document
            foreach (XElement propertyGroup in document.Root?.Elements("PropertyGroup")!) {
                XElement? iconElement = propertyGroup.Element("PackageIcon");

                // If <PackageIcon> does not exist, create it
                if (iconElement is null) {
                    iconElement = new XElement("PackageIcon", "icon.png");
                    propertyGroup.Add(iconElement);
                }
                else {
                    // Update the value to ensure it's "icon.png"
                    iconElement.Value = "icon.png";
                }

                // Look for ItemGroup containing Packable items
                XElement? packableItemGroup = document.Root?
                    .Elements("ItemGroup")
                    .FirstOrDefault(group => group.Elements("None")
                        .Any(item => item.Attribute("Pack")?.Value == "true"));

                if (packableItemGroup is null) {
                    // Create the ItemGroup if it doesn't exist
                    packableItemGroup = new XElement("ItemGroup");
                    document.Root?.Add(packableItemGroup);
                }

                // Check if the icon.png item already exists
                bool iconExists = packableItemGroup.Elements("None")
                    .Any(item => item.Attribute("Include")?.Value.EndsWith("icon.png") == true);

                if (iconExists) continue;

                IEnumerable<string> indents = Enumerable.Repeat(
                    "../",
                    1
                    + (IsEmptyFolderNameRegex.IsMatch(args.SourceFolder) ? 1 : 0)
                    + args.SourceFolder.Count(c => c is '/' or '\\')
                );
                string includeString = Path.Combine(string.Join(string.Empty, indents), args.IconFolder, "icon.png");

                // Add the icon.png reference if it doesn't exist
                var newIconElement = new XElement("None",
                    new XAttribute("Include", includeString),
                    new XAttribute("Pack", "true"),
                    new XAttribute("PackagePath", ""));
                packableItemGroup.Add(newIconElement);
            }
        }

        return new Success();
    }

    private static async Task<SuccessOrFailure> TryGetIcon(DownloadIconParameters parameters) {
        try {
            // Validate Origin
            if (string.IsNullOrWhiteSpace(parameters.Origin)) {
                return "Error: The origin of the icon is not specified.";
            }

            // Assume Origin could be either a URL or a file path
            bool isUrl = Uri.TryCreate(parameters.Origin, UriKind.Absolute, out Uri? uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            // Placeholder for the final icon's path
            string destinationPath = Path.Combine(parameters.Root, parameters.IconFolder, "icon.png");

            if (isUrl) {
                // Download the file from URL using HttpClient
                using var client = new HttpClient();
                Console.WriteLine($"Downloading icon from URL: {parameters.Origin}");

                using HttpResponseMessage response = await client.GetAsync(parameters.Origin);
                if (!response.IsSuccessStatusCode) {
                    return $"Error: Failed to download the icon. HTTP Status: {response.StatusCode}";
                }

                byte[] iconBytes = await response.Content.ReadAsByteArrayAsync();
                await File.WriteAllBytesAsync(destinationPath, iconBytes);

                Console.WriteLine($"Icon downloaded successfully to: {destinationPath}");
            }
            else {
                // Treat as file system path and copy the icon
                Console.WriteLine($"Copying icon from local path: {parameters.Origin}");

                if (!File.Exists(parameters.Origin)) {
                    return $"Error: The specified origin file does not exist: {parameters.Origin}";
                }

                File.Copy(parameters.Origin, destinationPath, true);
                Console.WriteLine($"Icon copied successfully to: {destinationPath}");
            }

            // If all operations succeed
            return new Success();
        }
        catch (Exception ex) {
            // Return any unexpected errors
            return $"Unexpected error: {ex.Message}";
        }
    }
}
