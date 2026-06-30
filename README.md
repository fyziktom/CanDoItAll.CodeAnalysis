# CanDoItAll.CodeAnalsis

`CanDoItAll.CodeAnalsis` is a .NET 10 code-analysis engine plus a desktop-large sandbox UI. It loads C# solutions with Roslyn/MSBuild, builds deterministic architecture snapshots, derives dependency/service/symbol/persistence facts, renders Markdown and Mermaid exports, and stores snapshots on disk.

The repository root and canonical solution intentionally keep the `CodeAnalsis` typo for compatibility with the transfer shape:

- Repository root: `CanDoItAll.CodeAnalsis`
- Canonical solution: `CanDoItAll.CodeAnalsis.slnx`
- Project and namespace family: `CanDoItAll.CodeAnalytics.*`
- Future host driver: `CanDoItAll.Mcp.CodeAnalytics`

## Packages

The reusable engine is published as library packages; the Web project is only a desktop sandbox app.

| Package | Role |
| --- | --- |
| `CanDoItAll.CodeAnalytics.Domain` | Immutable snapshot, fact, diagnostic, source, and export models. |
| `CanDoItAll.CodeAnalytics.Abstractions` | Public commands, queries, responses, enums, and `ICodeAnalyticsApplicationService`. |
| `CanDoItAll.CodeAnalytics.Workspace` | MSBuildWorkspace and Roslyn loading helpers. |
| `CanDoItAll.CodeAnalytics.Facts` | Static fact collectors for projects, symbols, DI, dependencies, and EF Core persistence metadata. |
| `CanDoItAll.CodeAnalytics.Analysis` | Findings and insight derivation over collected facts. |
| `CanDoItAll.CodeAnalytics.Rendering` | Markdown and Mermaid export rendering. |
| `CanDoItAll.CodeAnalytics.Storage` | File-system snapshot, recent-index, and export storage. |
| `CanDoItAll.CodeAnalytics.Application` | Engine facade for snapshot build and query workflows. |

`CanDoItAll.CodeAnalytics.Web`, `tools/*`, `tests/*`, and `tests/fixtures/*` are non-packable. See `reference/publishing-readiness.md`.

## Engine API

The main contract is `ICodeAnalyticsApplicationService` in `CanDoItAll.CodeAnalytics.Abstractions`.

Common workflows:

- `BuildSnapshotAsync(BuildArchitectureSnapshotCommand)` builds or loads a deterministic snapshot.
- `GetDashboardAsync`, `GetDependenciesAsync`, `GetServicesAsync`, `GetPersistenceAsync`, and `GetFindingsAsync` return snapshot views.
- `GetSolutionInventoryAsync`, `GetProjectInventoryAsync`, `GetDocumentSourceAsync`, and `GetDocumentSymbolsAsync` inspect project/document inventory.
- `SearchSymbolsAsync`, `GetSymbolDefinitionAsync`, `GetSymbolMembersAsync`, `GetSymbolImplementationsAsync`, and `GetSymbolReferencesAsync` power symbol exploration.
- `GetFocusedContextAsync` selects a bounded, prompt-oriented context bundle for a service, type, member, diagnostic, or text query.
- `GetExportsAsync`, `GetSnapshotAsync`, and `ListRecentSnapshotsAsync` expose stored outputs.

The Web project's `Program.RegisterServices` method is the current composition reference until a reusable DI extension is introduced. See `reference/public-api.md`.

## Static EF Scope

The EF work is static analysis, not runtime query tuning. `CanDoItAll.CodeAnalytics.Facts` can identify `DbContext` types, entity sets, relationships, model snapshot metadata, configuration signals, and diagnostics from source/symbols. It does not execute EF Core queries or claim N+1, `AsNoTracking`, split-query, SQL-shape, index, or client-evaluation guidance. See `reference/ef-analyzer-capabilities.md`.

## Desktop Sandbox

Run the sandbox app for large-screen local inspection:

```powershell
dotnet run --project .\src\CanDoItAll.CodeAnalytics.Web\CanDoItAll.CodeAnalytics.Web.csproj --urls http://127.0.0.1:5294
```

Optional environment variables:

- `CODE_ANALYTICS_DEFAULT_SOLUTION_PATH` sets the initial solution path.
- `CODE_ANALYTICS_OUTPUT_ROOT` sets the snapshot/export output root.

The sandbox is intentionally designed for desktop-large workflows. Small and medium responsive tuning is out of scope for this publishing wave. See `reference/desktop-sandbox.md`.

## Validation

Run the segmented release gate from the repository root:

```powershell
dotnet restore
dotnet build .\CanDoItAll.CodeAnalsis.slnx -warnaserror
dotnet test .\tests\CanDoItAll.CodeAnalytics.Tests.Architecture\CanDoItAll.CodeAnalytics.Tests.Architecture.csproj --no-build --blame-hang --blame-hang-timeout 60s --logger "console;verbosity=normal"
dotnet test .\tests\CanDoItAll.CodeAnalytics.Tests.Unit\CanDoItAll.CodeAnalytics.Tests.Unit.csproj --no-build --blame-hang --blame-hang-timeout 600s --logger "console;verbosity=normal"
dotnet test .\tests\CanDoItAll.CodeAnalytics.Tests.Integration\CanDoItAll.CodeAnalytics.Tests.Integration.csproj --no-build --blame-hang --blame-hang-timeout 600s --logger "console;verbosity=normal"
dotnet test .\tests\CanDoItAll.CodeAnalytics.Tests.Web\CanDoItAll.CodeAnalytics.Tests.Web.csproj --no-build --blame-hang --blame-hang-timeout 600s --logger "console;verbosity=normal"
.\eng\Validate-FileLengths.ps1
.\eng\Validate-SolutionStructure.ps1
```

The Roslyn-backed Unit, Integration, and Web tests are intentionally slower than architecture tests. Use `codex/validation-matrix.md` for the release matrix and optional commands.

## Packaging

Build first, then pack the release projects:

```powershell
dotnet build .\CanDoItAll.CodeAnalsis.slnx -warnaserror
.\eng\Pack-ReleaseProjects.ps1 -Configuration Debug -OutputPath .\.artifacts\packages -NoBuild
```

Inspect package contents before publishing. The package set must not contain Web assets, tests, fixtures, Codex bundle proof, local snapshot output, `.artifacts` content, or machine-local paths.

## Repository Docs

- `LICENSE` - MIT license.
- `SECURITY.md` - vulnerability reporting policy.
- `CONTRIBUTING.md` - contribution and validation guide.
- `architecture/adrs/` - accepted publishing-prep decisions.
- `reference/` - package, API, sandbox, EF, performance, and future-driver reference docs.
- `codex/bundles/CanDoItAll.CodeAnalsis.PublishPrepBundle` - execution bundle and proof artifacts for this publishing-prep wave.
