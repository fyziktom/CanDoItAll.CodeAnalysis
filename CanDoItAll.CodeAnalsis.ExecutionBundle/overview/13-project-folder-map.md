# Project folder map

## Production projects

- ### `src/CanDoItAll.CodeAnalytics.Abstractions`
Suggested folders:
- Requests/\n- Responses/\n- Progress/\n- Queries/\n- Options/
- ### `src/CanDoItAll.CodeAnalytics.Domain`
Suggested folders:
- Snapshot/\n- Facts/\n- Insights/\n- Diagnostics/\n- Identifiers/\n- Sources/\n- Exports/
- ### `src/CanDoItAll.CodeAnalytics.Workspace`
Suggested folders:
- Loading/\n- Normalization/\n- Inventory/
- ### `src/CanDoItAll.CodeAnalytics.Facts`
Suggested folders:
- Symbols/\n- Documentation/\n- Dependencies/\n- Services/\n- Persistence/
- ### `src/CanDoItAll.CodeAnalytics.Analysis`
Suggested folders:
- Modules/\n- Graphs/\n- Rules/\n- Findings/\n- Metrics/
- ### `src/CanDoItAll.CodeAnalytics.Rendering`
Suggested folders:
- Markdown/\n- Mermaid/\n- Exports/
- ### `src/CanDoItAll.CodeAnalytics.Storage`
Suggested folders:
- Snapshots/\n- Recent/\n- Caching/\n- Paths/
- ### `src/CanDoItAll.CodeAnalytics.Application`
Suggested folders:
- Services/\n- Commands/\n- Queries/\n- Composition/
- ### `src/CanDoItAll.CodeAnalytics.Web`
Suggested folders:
- Components/\n- Pages/\n- State/\n- Features/

## Test projects

- `tests/CanDoItAll.CodeAnalytics.Tests.Support` — Fixture builders, shared helpers, and test assets.
- `tests/CanDoItAll.CodeAnalytics.Tests.Unit` — Deterministic logic and contract tests.
- `tests/CanDoItAll.CodeAnalytics.Tests.Integration` — Fixture-solution and file-system integration tests.
- `tests/CanDoItAll.CodeAnalytics.Tests.Web` — SSR route and UI flow tests.
- `tests/CanDoItAll.CodeAnalytics.Tests.Architecture` — Layer-boundary, naming-map, and future MCP seam protection tests.

## Root documentation assets

- `architecture/adrs/`
- `codex/README.md`
- optional `.codex/agents/`
- `tests/fixtures/`

## Do not create by default

- catch-all `Helpers/`
- giant `Models/` folders containing unrelated types
- MCP driver project in this first standalone wave
