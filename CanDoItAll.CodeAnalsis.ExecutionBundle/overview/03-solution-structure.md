# Solution structure

## Naming strategy

### Canonical root names
- repository root: `CanDoItAll.CodeAnalsis`
- canonical solution file: `CanDoItAll.CodeAnalsis.slnx`
- optional compatibility solution file: `CanDoItAll.CodeAnalsis.sln` **only** if some toolchain blocks on `.slnx`

### Namespace and assembly family
Use the **correctly spelled** namespace and assembly family:
- `CanDoItAll.CodeAnalytics.*`

### Future host-repo driver
Reserve the future host-repo driver name:
- `CanDoItAll.Mcp.CodeAnalytics`

## Why the typo/correct-spelling split is intentional

The user explicitly wants the repo/solution identity to stay **`CodeAnalsis`** for transfer convenience.
That typo should not leak into the reusable project and namespace family because it would become long-term technical debt.
Therefore:

- repo + solution identity => `CodeAnalsis`
- project/namespace/assembly identity => `CodeAnalytics`
- future host-repo driver => `CanDoItAll.Mcp.CodeAnalytics`

## Proposed production projects

- `src/CanDoItAll.CodeAnalytics.Abstractions` — Transport-agnostic requests, responses, progress contracts, options, and query DTOs.
- `src/CanDoItAll.CodeAnalytics.Domain` — Canonical snapshot model, identifiers, diagnostics, facts, insights, and export metadata records.
- `src/CanDoItAll.CodeAnalytics.Workspace` — MSBuild/Roslyn workspace bootstrapping and solution/project/document inventory loading.
- `src/CanDoItAll.CodeAnalytics.Facts` — Collectors for symbols, XML docs, namespaces, DI, EF Core, and other static facts.
- `src/CanDoItAll.CodeAnalytics.Analysis` — Graph algorithms, module grouping, risk rules, and insight generation.
- `src/CanDoItAll.CodeAnalytics.Rendering` — Markdown writers, Mermaid renderers, and export packaging logic.
- `src/CanDoItAll.CodeAnalytics.Storage` — Snapshot persistence, recent-run indexing, cache metadata, and deterministic file layout.
- `src/CanDoItAll.CodeAnalytics.Application` — Orchestrator/services that compose the pipeline and expose future MCP-ready operations.
- `src/CanDoItAll.CodeAnalytics.Web` — Simple SSR-first Blazor Web UI for running analyses and browsing results.

## Proposed test projects

- `tests/CanDoItAll.CodeAnalytics.Tests.Support` — Fixture builders, shared helpers, and test assets.
- `tests/CanDoItAll.CodeAnalytics.Tests.Unit` — Deterministic logic and contract tests.
- `tests/CanDoItAll.CodeAnalytics.Tests.Integration` — Fixture-solution and file-system integration tests.
- `tests/CanDoItAll.CodeAnalytics.Tests.Web` — SSR route and UI flow tests.
- `tests/CanDoItAll.CodeAnalytics.Tests.Architecture` — Layer-boundary, naming-map, and future MCP seam protection tests.

## Root folders

- `src/`
- `tests/`
- `eng/`
- `architecture/`
- `codex/`
- `.codex/` (optional placeholders or agents later)
- `docs/` (optional general docs if needed)

## Alignment with the current CanDoItAll repo

The current host repo uses:
- `CanDoItAll.slnx`
- `global.json` pinned to **10.0.200**
- `Directory.Build.props`
- `src/`, `tests/`, `tools/`, `codex/`, `.codex/`, `architecture/`

The standalone repo should mirror that overall shape where it improves later portability, but keep its own neutral libraries and avoid copying host-only runtime infrastructure.

## Future driver project shape inside the host repo

When the code is transplanted into the main CanDoItAll repo, the future MCP project should look like:

- `src/CanDoItAll.Mcp.CodeAnalytics/Program.cs`
- `src/CanDoItAll.Mcp.CodeAnalytics/Configuration/`
- `src/CanDoItAll.Mcp.CodeAnalytics/Tools/`
- `src/CanDoItAll.Mcp.CodeAnalytics/Coordination/` and/or `Runtime/`
- `tests/CanDoItAll.Mcp.CodeAnalytics.Tests/`

The standalone repo should make that project thin by design.
