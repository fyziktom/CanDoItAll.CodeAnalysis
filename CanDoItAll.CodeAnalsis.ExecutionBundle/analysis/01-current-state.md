# Current state

## Repo shape

- Solution: `C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.slnx`
- Main source projects:
  - `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Abstractions`
  - `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Analysis`
  - `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Application`
  - `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Domain`
  - `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Facts`
  - `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Rendering`
  - `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Storage`
  - `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Web`
  - `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Workspace`

## Bundle state

- The original bundle shipped with an older folder contract and older subbundle document format.
- The reopened work requires a true `initiative` bundle with traceability, dependency gates, inventories, templates, and an execution report compatible with the current workflow validator.
- The bundle validator failed before repair because the core planning folders and README contracts were missing.

## Hotspots by file length

- `495` lines: `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Facts\Persistence\PersistenceFactCollector.cs`
- `445` lines: `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Web\wwwroot\app.css`
- `415` lines: `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Facts\Persistence\PersistenceSyntaxExplorer.cs`
- `412` lines: `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Application\Services\CodeAnalyticsApplicationService.Build.cs`
- `385` lines: `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Facts\Dependencies\DependencyFactCollector.cs`
- `359` lines: `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Facts\Symbols\SymbolFactsCollector.cs`
- `344` lines: `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Facts\Persistence\PersistenceFactCollector.Helpers.cs`

## Architecture gaps

- `PersistenceFactCollector` still mixes project scanning, relationship merging, entity resolution, diagnostics, and output assembly.
- `PersistenceSyntaxExplorer` still owns too many unrelated EF heuristics in one place.
- `DependencyFactCollector` mixes module graph assembly, type relationship heuristics, and duplicate resolution.
- `CodeAnalyticsApplicationService` partials are better than a monolith, but the public query surface still lacks a dedicated focused-context pipeline.
- The rendering layer still emits a single global class diagram export that is technically valid but weak for large-solution orientation.
- The UI supports dashboard, persistence, services, findings, exports, and type search, but it does not yet support a centered context exploration workflow.
- The domain model has type and entity relationship facts, but no canonical member relationship graph yet.

## Usefulness assessment from host run

- The ER output is now helpful enough to surface real schema edges in CanDoItAll.
- The class diagram is still too broad at whole-solution scope.
- SharpTools remains stronger for exact source truth.
- The standalone snapshot is already stronger for one-shot repo orientation and cross-cutting counts.
- The missing bridge is a focused context view that narrows the snapshot to the trouble path developers actually follow.
