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

## Focused-context comparison findings from the new reopen

- Database case:
  - `AppDbContext` with project scope `CanDoItAll.Infrastructure` and tag `Db` resolved the expected seed.
  - The output was still too noisy: `622` selected lines, `8` files, and `15` blocks, including near-full-file excerpts in `StorageCatalogService.cs` and `BackgroundJobs.cs`.
  - The strongest signal is that constructor-heavy seeding and type-only file fallbacks are over-expanding.
- Common-helper case:
  - `IClock` with whole-solution scope and tag `Service` did not merely return a noisy result.
  - It hard-failed with duplicate normalized source-path handling around `Microsoft.NET.Test.Sdk.Program.cs`.
  - This is now a correctness defect in the focused-context foundation, not only a tuning issue.
- UI case:
  - `CanvasSceneHost` with project scope `CanDoItAll.Components.CanvasLib` and tag `Ui` produced a strong first-pass result: `98` selected lines across `3` files.
  - This is the preservation case for the next implementation pass. Noise tightening must not regress this narrower UI flow.

## Helper-precision findings from the newest reopen

- The residual helper-noise problem is now narrower and better understood than the previous reopen:
  - seed resolution for helpers is no longer the main defect,
  - the remaining issue is that high-fan-in helpers still flow through the same undirected traversal used for trouble-path exploration.
- The host `IClock` footprint is wide enough to require a different mode:
  - `62` source matches,
  - `41` source files,
  - `14` source projects,
  - strongest spread in `CanDoItAll.Modules.Workbench`, `CanDoItAll.Infrastructure`, `CanDoItAll.Modules.Automation`, and `CanDoItAll.Modules.CrmHr`.
- SharpTools remains more surgical for helpers because it separates:
  - definition lookup,
  - implementation lookup,
  - and usage search,
  instead of collapsing them into one automatically expanded bundle.
- The next pass therefore needs:
  - explicit helper-oriented intent or precision handling,
  - directional traversal instead of always exploring both directions,
  - sampled or summarized consumers instead of trying to load the full helper neighborhood into the main excerpt payload.
