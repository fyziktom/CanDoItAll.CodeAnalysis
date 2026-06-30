using System.Diagnostics;
using CanDoItAll.CodeAnalytics.Abstractions;
using CanDoItAll.CodeAnalytics.Abstractions.Queries;
using CanDoItAll.CodeAnalytics.Domain.Facts;
using CanDoItAll.CodeAnalytics.Domain.Snapshot;
using CanDoItAll.CodeAnalytics.Domain.Sources;
using CanDoItAll.CodeAnalytics.Storage.Paths;
using CanDoItAll.CodeAnalytics.Storage.Snapshots;
using CanDoItAll.CodeAnalytics.Tests.Support;

namespace CanDoItAll.CodeAnalytics.Tests.Unit;

public sealed class ApplicationSafetyFacts {
    [Fact]
    public async Task Application_regex_symbol_search_handles_adversarial_patterns_without_throwing() {
        using var output = new TemporaryDirectoryScope();
        var original = SampleSnapshotFactory.Create();
        var longDisplayName = $"{new string('a', 8192)}!";
        var type = original.Facts.Types[0] with {
            DisplayName = longDisplayName,
            XmlSummary = null,
            Source = new SourceReference("src/LongType.cs", 1, 1),
        };
        var snapshot = original with {
            Facts = original.Facts with {
                Types = [type],
                Members = [],
            },
        };
        await StoreSnapshotAsync(snapshot, output.Path);

        var service = ApplicationServiceFactory.Create(output.Path);
        var stopwatch = Stopwatch.StartNew();
        var response = await service.SearchSymbolsAsync(
            new SymbolSearchQuery(
                snapshot.SnapshotId,
                SearchText: "(a+)+$",
                SearchMode: SymbolSearchMode.Regex));

        stopwatch.Stop();
        Assert.NotNull(response);
        Assert.Null(response!.ValidationError);
        Assert.Empty(response.Results);
        Assert.True(stopwatch.Elapsed < TimeSpan.FromSeconds(5), $"Regex search took {stopwatch.Elapsed}.");
    }

    [Fact]
    public async Task Application_document_source_rejects_paths_outside_workspace_root() {
        using var workspace = new TemporaryDirectoryScope();
        using var outside = new TemporaryDirectoryScope();
        using var output = new TemporaryDirectoryScope();
        var solutionPath = Path.Combine(workspace.Path, "Fixture.Shop.slnx");
        await File.WriteAllTextAsync(solutionPath, string.Empty);

        var outsidePath = Path.Combine(outside.Path, "Outside.cs");
        await File.WriteAllTextAsync(outsidePath, "public sealed class Outside { }");
        var escapingPath = Path.GetRelativePath(workspace.Path, outsidePath);
        var snapshot = CreateDocumentSnapshot(solutionPath, escapingPath, lineCount: 1);
        await StoreSnapshotAsync(snapshot, output.Path);

        var service = ApplicationServiceFactory.Create(output.Path);
        var response = await service.GetDocumentSourceAsync(new DocumentQuery(snapshot.SnapshotId, DocumentId: "doc-safety"));

        Assert.Null(response);
    }

    [Fact]
    public async Task Application_document_source_rejects_files_above_public_read_limit() {
        using var workspace = new TemporaryDirectoryScope();
        using var output = new TemporaryDirectoryScope();
        var solutionPath = Path.Combine(workspace.Path, "Fixture.Shop.slnx");
        var sourceDirectory = Path.Combine(workspace.Path, "src");
        var sourcePath = Path.Combine(sourceDirectory, "Large.cs");
        Directory.CreateDirectory(sourceDirectory);
        await File.WriteAllTextAsync(solutionPath, string.Empty);
        await File.WriteAllTextAsync(sourcePath, new string('x', 2_100_000));

        var snapshot = CreateDocumentSnapshot(solutionPath, "src/Large.cs", lineCount: 1);
        await StoreSnapshotAsync(snapshot, output.Path);

        var service = ApplicationServiceFactory.Create(output.Path);
        var response = await service.GetDocumentSourceAsync(new DocumentQuery(snapshot.SnapshotId, DocumentId: "doc-safety"));

        Assert.Null(response);
    }

    private static ArchitectureSnapshot CreateDocumentSnapshot(
        string solutionPath,
        string documentPath,
        int lineCount) {
        var original = SampleSnapshotFactory.Create();
        return original with {
            Request = original.Request with {
                SolutionPath = solutionPath,
            },
            Facts = original.Facts with {
                Documents = [
                    new DocumentFact(
                        "doc-safety",
                        "proj-app",
                        documentPath,
                        Path.GetFileName(documentPath),
                        lineCount),
                ],
            },
        };
    }

    private static async Task StoreSnapshotAsync(ArchitectureSnapshot snapshot, string outputPath) {
        var repository = new FileSnapshotRepository(new SnapshotJsonSerializer());
        var pathResolver = new SnapshotPathResolver(outputPath);
        var requestHash = repository.ComputeRequestHash(snapshot.Request, snapshot.GeneratorVersion, snapshot.SchemaVersion);
        await repository.StoreAsync(pathResolver, snapshot, requestHash, [], CancellationToken.None);
    }
}
