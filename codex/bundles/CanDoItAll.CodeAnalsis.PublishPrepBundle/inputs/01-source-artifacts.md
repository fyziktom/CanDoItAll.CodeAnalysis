# Source Artifacts

## Raw And Repo Sources

- `bundle://inputs/00-original-request.md`
- `repo://README.md`
- `repo://CanDoItAll.CodeAnalsis.slnx`
- `repo://Directory.Build.props`
- `repo://global.json`
- `repo://eng/Validate-FileLengths.ps1`
- `repo://eng/Validate-SolutionStructure.ps1`
- `repo://architecture/adrs/README.md`
- `repo://codex/README.md`
- `repo://reference/compatibility-matrix.md`
- `repo://reference/reuse-later-vs-do-not-duplicate-now.md`
- `repo://reference/current-candoitall-mcp-context.md`
- `repo://reference/tool-surface-proposal.json`
- `repo://reference/CanDoItAll.Mcp.CodeAnalytics.settings.example.json`

## Primary Source Hotspots Inspected

- `repo://src/CanDoItAll.CodeAnalytics.Application/Services/CodeAnalyticsApplicationService.Context.Strategy.cs`
- `repo://src/CanDoItAll.CodeAnalytics.Application/Services/CodeAnalyticsApplicationService.Symbols.cs`
- `repo://src/CanDoItAll.CodeAnalytics.Application/Services/CodeAnalyticsApplicationService.Context.SeedResolution.cs`
- `repo://src/CanDoItAll.CodeAnalytics.Application/Services/CodeAnalyticsApplicationService.Symbols.Source.cs`
- `repo://src/CanDoItAll.CodeAnalytics.Facts/Persistence/PersistenceFactCollector.cs`
- `repo://src/CanDoItAll.CodeAnalytics.Facts/Persistence/PersistenceSyntaxExplorer.ModelSnapshots.cs`
- `repo://src/CanDoItAll.CodeAnalytics.Rendering/Exports/ExportBundleBuilder.cs`
- `repo://src/CanDoItAll.CodeAnalytics.Storage/Snapshots/FileSnapshotRepository.cs`
- `repo://src/CanDoItAll.CodeAnalytics.Web/Program.cs`
- `repo://src/CanDoItAll.CodeAnalytics.Web/Components/Pages/ContextLab.razor`
- `repo://src/CanDoItAll.CodeAnalytics.Web/Components/Pages/Snapshots/Context.razor`
- `repo://src/CanDoItAll.CodeAnalytics.Web/Components/Pages/Snapshots/Symbols.razor`
- `repo://src/CanDoItAll.CodeAnalytics.Web/wwwroot/styles/base.css`
- `repo://src/CanDoItAll.CodeAnalytics.Web/wwwroot/styles/snapshots.css`

## EF Fixture Sources Inspected

- `repo://tests/fixtures/Fixture.Shop/src/Fixture.Shop.Infrastructure/Persistence/EfRepository.cs`
- `repo://tests/fixtures/Fixture.Shop/src/Fixture.Shop.Infrastructure/Persistence/ShopDbContext.cs`
- `repo://tests/fixtures/Fixture.Shop/src/Fixture.Shop.Infrastructure/Persistence/ReportingDbContext.cs`
- `repo://tests/fixtures/Fixture.Shop/src/Fixture.Shop.Application/Orders/OrderService.cs`

## Preparation Commands Run

- `git status --short`
- `dotnet --info`
- `dotnet build .\CanDoItAll.CodeAnalsis.slnx -warnaserror`
- `dotnet test .\CanDoItAll.CodeAnalsis.slnx --no-build`
- `.\eng\Validate-SolutionStructure.ps1`
- `.\eng\Validate-FileLengths.ps1`
- `rg` scans for file size, project metadata, docs, performance recipes, EF usage, and UI/component hotspots.
