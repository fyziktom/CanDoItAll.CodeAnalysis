using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace CanDoItAll.CodeAnalytics.Domain.Identifiers;

public static partial class StableId {
    public static string ForDocument(string value) {
        return Create("doc", value);
    }

    public static string ForEntity(string value) {
        return Create("ent", value);
    }

    public static string ForFinding(string value) {
        return Create("finding", value);
    }

    public static string ForModule(string value) {
        return Create("mod", value);
    }

    public static string ForNamespace(string value) {
        return Create("ns", value);
    }

    public static string ForProject(string value) {
        return Create("proj", value);
    }

    public static string ForServiceRegistration(string value) {
        return Create("svc", value);
    }

    public static string ForSnapshot(string value) {
        return Create("snap", value);
    }

    public static string ForType(string value) {
        return Create("type", value);
    }

    public static string ForMember(string value) {
        return Create("member", value);
    }

    public static string ForDependency(string value) {
        return Create("dep", value);
    }

    public static string ForDbContext(string value) {
        return Create("dbctx", value);
    }

    public static string ToHash(string value) {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(value));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    private static string Create(string prefix, string value) {
        var slug = CleanupRegex().Replace(value.ToLowerInvariant(), "-").Trim('-');
        if (string.IsNullOrWhiteSpace(slug)) {
            slug = "empty";
        }

        if (slug.Length > 64) {
            slug = slug[..64].Trim('-');
        }

        return $"{prefix}-{slug}";
    }

    [GeneratedRegex("[^a-z0-9]+", RegexOptions.Compiled)]
    private static partial Regex CleanupRegex();
}
