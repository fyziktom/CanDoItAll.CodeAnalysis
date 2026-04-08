# Source artifacts

- Repository under repair: `C:\repositories\CanDoItAll.CodeAnalsis`
- Read-only compatibility host: `C:\repositories\CanDoItAll`
- Existing bundle root: `C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.ExecutionBundle`
- Existing workbook: `C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.ExecutionBundle\spreadsheets\CanDoItAll.CodeAnalsis.ExecutionBacklog.xlsx`
- Existing execution evidence: `C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.ExecutionBundle\reviews\01-execution-report.md`
- Existing refactor evidence: `C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.ExecutionBundle\reviews\02-refactor-report.md`
- Existing review evidence: `C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.ExecutionBundle\reviews\03-review-report.md`
- Host validation snapshot: `C:\repositories\CanDoItAll.CodeAnalsis\output\playwright\host-analysis-v2\snapshots\snap-20260407205715-123ebd81`
- Host validation dashboard screenshot: `C:\repositories\CanDoItAll.CodeAnalsis\output\playwright\host-analysis-v2-dashboard.png`
- Host validation exports screenshot: `C:\repositories\CanDoItAll.CodeAnalsis\output\playwright\host-analysis-v2-exports.png`
- Mermaid render proof: `class-diagram.svg` and `er-diagram.svg` under the host validation snapshot exports folder
- Bundle validator script: `C:\Users\lucys\.codex\skills\candoitall-bundle-preparation\scripts\validate_bundle.py`
- Bundle workflow skill: `C:\Users\lucys\.codex\skills\candoitall-bundle-workflow\SKILL.md`

## Real repo observations captured before repair

- Bundle validator failed because the bundle used an older structure without `inputs`, `analysis`, `requirements`, `plan`, `traceability`, or `shared-prompts`.
- The largest current source hotspots are:
  - `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Facts\Persistence\PersistenceFactCollector.cs`
  - `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Facts\Persistence\PersistenceSyntaxExplorer.cs`
  - `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Facts\Dependencies\DependencyFactCollector.cs`
  - `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Application\Services\CodeAnalyticsApplicationService.Build.cs`
  - `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Facts\Symbols\SymbolFactsCollector.cs`
  - `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Web\wwwroot\app.css`
- Host snapshot metrics on `CanDoItAll.slnx` currently show:
  - `40` projects
  - `2083` types
  - `239` service registrations
  - `81` entities
  - `5144` type relationships
  - `5` entity relationships
  - `638` findings
  - `362` diagnostics
