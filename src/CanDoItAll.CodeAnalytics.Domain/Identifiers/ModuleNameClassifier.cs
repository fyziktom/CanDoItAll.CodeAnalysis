namespace CanDoItAll.CodeAnalytics.Domain.Identifiers;

public static class ModuleNameClassifier {
    public static string GetModuleName(string projectName, string namespaceName) {
        if (string.IsNullOrWhiteSpace(namespaceName)) {
            return projectName;
        }

        if (!namespaceName.StartsWith(projectName, StringComparison.Ordinal)) {
            return projectName;
        }

        var remainder = namespaceName[projectName.Length..].Trim('.');
        if (string.IsNullOrWhiteSpace(remainder)) {
            return projectName;
        }

        var firstSegment = remainder.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .FirstOrDefault();
        if (string.IsNullOrWhiteSpace(firstSegment)) {
            return projectName;
        }

        return $"{projectName}.{firstSegment}";
    }
}
