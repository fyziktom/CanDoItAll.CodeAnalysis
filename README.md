# CanDoItAll.CodeAnalysis

[![CI](https://github.com/fyziktom/CanDoItAll.CodeAnalysis/actions/workflows/ci.yml/badge.svg?branch=main&event=push)](https://github.com/fyziktom/CanDoItAll.CodeAnalysis/actions/workflows/ci.yml)
[![Analysis version](https://img.shields.io/nuget/v/CanDoItAll.CodeAnalytics.Analysis.svg?logo=nuget&label=Analysis)](https://www.nuget.org/packages/CanDoItAll.CodeAnalytics.Analysis)
[![Analysis downloads](https://img.shields.io/nuget/dt/CanDoItAll.CodeAnalytics.Analysis.svg?logo=nuget&label=Analysis%20downloads)](https://www.nuget.org/packages/CanDoItAll.CodeAnalytics.Analysis)
[![Rendering version](https://img.shields.io/nuget/v/CanDoItAll.CodeAnalytics.Rendering.svg?logo=nuget&label=Rendering)](https://www.nuget.org/packages/CanDoItAll.CodeAnalytics.Rendering)
[![Rendering downloads](https://img.shields.io/nuget/dt/CanDoItAll.CodeAnalytics.Rendering.svg?logo=nuget&label=Rendering%20downloads)](https://www.nuget.org/packages/CanDoItAll.CodeAnalytics.Rendering)
[![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/download/dotnet/10.0)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

`CanDoItAll.CodeAnalysis` is a .NET 10 code-analysis engine plus a desktop-large
sandbox. It loads C# solutions with Roslyn/MSBuild, builds deterministic architecture
snapshots, derives dependency/service/symbol/persistence facts, renders Markdown and
Mermaid exports, and stores snapshots on disk.

The canonical solution retains the historical `CodeAnalsis` typo as a documented
compatibility exception:

- Repository and source URL: `CanDoItAll.CodeAnalysis`
- Canonical solution: `CanDoItAll.CodeAnalsis.slnx`
- Project and namespace family: `CanDoItAll.CodeAnalytics.*`
- Future host driver: `CanDoItAll.Mcp.CodeAnalytics`

## Ownership

This repository owns the reusable CodeAnalytics engine, its public NuGet packages, tests,
local analysis tools, release adapter, and non-packable desktop sandbox.

It does not own the future MCP host driver or `CanDoItAll.Components`; the sandbox
consumes the published component package from nuget.org.

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

The sandbox references `CanDoItAll.Components.BaseLib` `0.1.15` from nuget.org through
the root `NuGet.config`. It uses the shared theme, viewport layout, side menu, page
scaffolds, navigation, sections, forms, metrics, feedback, and overlay hosts.

## Validation

Run the segmented release gate from the repository root:

```powershell
dotnet restore .\CanDoItAll.CodeAnalsis.slnx --configfile .\NuGet.config
dotnet build .\CanDoItAll.CodeAnalsis.slnx --configuration Release --no-restore -warnaserror
.\tools\deployment\nugets\Build-NuGets.ps1 -Configuration Release -NoRestore
```

The canonical adapter runs the Architecture, Unit, Integration, and Web projects
sequentially, validates repository structure and file lengths, then packs all eight
shipping libraries. The Roslyn-backed suites are intentionally slower than architecture
tests. Use `codex/validation-matrix.md` for the expanded matrix.

## Packaging

Preview or execute the repository-owned package adapter:

```powershell
.\tools\deployment\nugets\Build-NuGets.ps1 -WhatIf
.\tools\deployment\nugets\Build-NuGets.ps1 -Version 0.1.5
.\tools\deployment\nugets\Build-NuGets.ps1 -Version 0.2.0 -PrereleaseSuffix '-preview.1'
.\tools\deployment\nugets\Build-NuGets.ps1 -Configuration Release -OutputDirectory .\artifacts\packages
```

When `-OutputDirectory` is omitted, the adapter creates
`artifacts/packages/<version>_<timestamp>`. An explicit output directory remains exact
for CI and cross-repository orchestration; add `-CreateRunDirectory` to create the same
versioned child beneath an explicit output root.

This command builds packages but never publishes them. Inspect the `.nuspec`, embedded
`LICENSE`, README, repository metadata, and archive contents before pushing a package.
The package set must not contain Web assets, tests, fixtures, Codex bundle proof, local
snapshot output, artifacts, or machine-local paths.

## Repository Docs

- `LICENSE` - unmodified MIT License.
- `SECURITY.md` - vulnerability reporting policy.
- `CONTRIBUTING.md` - contribution and validation guide.
- `docs/repository-standards.md` - shared-standard adoption and compatibility exceptions.
- `architecture/adrs/` - accepted publishing-prep decisions.
- `reference/` - package, API, sandbox, EF, performance, and future-driver reference docs.
- `codex/bundles/CanDoItAll.CodeAnalsis.PublishPrepBundle` - execution bundle and proof artifacts for this publishing-prep wave.

## License And Contributions

This repository uses the [MIT License](LICENSE).

Code contributions are limited to partners approved by the maintainer. See
[CONTRIBUTING.md](CONTRIBUTING.md) and contact the `fyziktom` account on LinkedIn before
preparing or opening a pull request.
