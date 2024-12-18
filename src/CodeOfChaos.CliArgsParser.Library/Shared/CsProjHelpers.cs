// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Xml;
using System.Xml.Linq;

namespace CodeOfChaos.CliArgsParser.Library.Shared;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public static class CsProjHelpers {
    public static string[] AsProjectPaths(string root, string sourcefolder, string[] projectNames) {
        return projectNames.Select(x => Path.Combine(root, sourcefolder, x, x + ".csproj")).ToArray();
    }
    
    public static async IAsyncEnumerable<XDocument> GetProjectFiles(string[] projectPaths) {
        var settings = new XmlWriterSettings {
            Indent = true,
            IndentChars = "    ",
            Async = true,
            OmitXmlDeclaration = true
        };
        
        foreach (string path in projectPaths) {
            if (!File.Exists(path)) {
                throw new FileNotFoundException($"Could not find project file {path}");
            }

            XDocument document;
            await using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true)) {
                document = await XDocument.LoadAsync(stream, LoadOptions.PreserveWhitespace, CancellationToken.None);
            }
            
            yield return document;
            
            await using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true)) {
                await using var writer = XmlWriter.Create(stream, settings);
                document.Save(writer);
            }
        }
    }
}
