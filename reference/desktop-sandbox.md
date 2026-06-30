# Desktop Sandbox Reference

`CanDoItAll.CodeAnalytics.Web` is a local inspection app for large desktop screens. It is not a reusable package and is not tuned for small or medium responsive layouts in this publishing wave.

## Run

```powershell
dotnet run --project .\src\CanDoItAll.CodeAnalytics.Web\CanDoItAll.CodeAnalytics.Web.csproj --urls http://127.0.0.1:5294
```

Optional environment variables:

- `CODE_ANALYTICS_DEFAULT_SOLUTION_PATH` - initial solution or project path.
- `CODE_ANALYTICS_OUTPUT_ROOT` - snapshot, recent-index, and export output root.

## Main Routes

| Route | Purpose |
| --- | --- |
| `/` | Build a snapshot and list recent snapshots. |
| `/operations/{id}` | Observe snapshot build progress and diagnostics. |
| `/snapshots/{id}` | Dashboard and navigation tabs. |
| `/snapshots/{id}/dependencies` | Project/module/type dependencies. |
| `/snapshots/{id}/services` | Dependency-injection registrations. |
| `/snapshots/{id}/persistence` | Static EF Core persistence facts. |
| `/snapshots/{id}/symbols` | Symbol search, details, members, implementations, and references. |
| `/snapshots/{id}/context` | Focused context for a selected service/type/member. |
| `/context-lab` | Free-form focused-context query lab. |
| `/snapshots/{id}/exports` | Markdown, Mermaid, and JSON exports. |

## Publishing Scope

The sandbox is intentionally non-packable. Reusable consumers should use `CanDoItAll.CodeAnalytics.Application` and `ICodeAnalyticsApplicationService` instead of coupling to Web components.

## Browser Proof

Publishing-prep desktop proof used a `1600x1000` viewport and captured home, operation details, dashboard, context lab, focused context, symbols search/detail, exports, and persistence routes. See `codex/bundles/CanDoItAll.CodeAnalsis.PublishPrepBundle/proof/SB06/browser`.
