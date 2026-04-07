using CanDoItAll.CodeAnalytics.Domain.Exports;
using CanDoItAll.CodeAnalytics.Domain.Facts;
using CanDoItAll.CodeAnalytics.Domain.Insights;
using CanDoItAll.CodeAnalytics.Domain.Snapshot;
using CanDoItAll.CodeAnalytics.Storage.Snapshots;
using CanDoItAll.CodeAnalytics.Tests.Support;

namespace CanDoItAll.CodeAnalytics.Tests.Unit;

public sealed class SerializationFacts {
    private readonly SnapshotJsonSerializer _serializer = new();

    [Fact]
    public void Serialization_round_trips_the_rich_snapshot() {
        var snapshot = SampleSnapshotFactory.Create();

        var json = _serializer.Serialize(snapshot);
        var roundTrip = _serializer.DeserializeSnapshot(json);

        SnapshotAssert.Equal(snapshot, roundTrip);
        GoldenFileAssert.EqualToFile("snapshots/rich-snapshot.json", json);
    }

    [Fact]
    public void Serialization_preserves_the_minimal_snapshot_shape() {
        var snapshot = new ArchitectureSnapshot(
            "1.1.0",
            "0.1.0",
            "snap-minimal",
            DateTimeOffset.Parse("2026-04-07T00:00:00Z"),
            new AnalysisRequest("/repo/minimal.slnx", [], [], false, false, false, false, false),
            new ArchitectureFacts(
                new SolutionFact("Minimal", "/repo/minimal.slnx", 0, 0),
                [],
                [],
                [],
                [],
                [],
                [],
                [],
                [],
                [],
                []),
            new ArchitectureInsights(
                new RiskSummaryInsight(0, 0, 0, 0, 0, 0, 0, 0),
                [],
                [],
                [],
                []),
            new ArchitectureExports([new ExportArtifact(ExportArtifactKind.SnapshotJson, "snapshot.json", "Snapshot JSON", "Canonical architecture snapshot.")]),
            []);

        var json = _serializer.Serialize(snapshot);
        GoldenFileAssert.EqualToFile("snapshots/minimal-snapshot.json", json);
    }
}
