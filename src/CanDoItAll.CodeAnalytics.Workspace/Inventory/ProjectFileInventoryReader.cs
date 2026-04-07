using System.Xml.Linq;

namespace CanDoItAll.CodeAnalytics.Workspace.Inventory;

public sealed class ProjectFileInventoryReader {
    public ProjectFileInventory Read(string projectPath) {
        if (!File.Exists(projectPath)) {
            return new ProjectFileInventory([], [], []);
        }

        var document = XDocument.Load(projectPath, LoadOptions.None);
        var targetFrameworks = document.Descendants("TargetFrameworks")
            .SelectMany(node => node.Value.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            .Concat(
                document.Descendants("TargetFramework")
                    .Select(node => node.Value)
                    .Where(value => !string.IsNullOrWhiteSpace(value)))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(value => value, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var projectReferences = document.Descendants("ProjectReference")
            .Select(node => node.Attribute("Include")?.Value)
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => Path.GetFullPath(Path.Combine(Path.GetDirectoryName(projectPath)!, value!)))
            .OrderBy(value => value, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var packageReferences = document.Descendants("PackageReference")
            .Select(node => node.Attribute("Include")?.Value)
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => value!)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(value => value, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return new ProjectFileInventory(targetFrameworks, projectReferences, packageReferences);
    }
}
