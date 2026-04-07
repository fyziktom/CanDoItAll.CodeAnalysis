using CanDoItAll.CodeAnalytics.Domain.Snapshot;
using CanDoItAll.CodeAnalytics.Storage.Snapshots;

namespace CanDoItAll.CodeAnalytics.Tests.Support;

public static class SnapshotAssert {
    private static readonly SnapshotJsonSerializer Serializer = new();

    public static void Equal(ArchitectureSnapshot expected, ArchitectureSnapshot actual) {
        var expectedJson = Serializer.Serialize(expected);
        var actualJson = Serializer.Serialize(actual);
        if (!string.Equals(expectedJson, actualJson, StringComparison.Ordinal)) {
            throw new InvalidOperationException("Snapshot JSON mismatch.");
        }
    }
}
