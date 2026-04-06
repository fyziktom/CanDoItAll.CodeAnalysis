# Analysis pipeline

## Stage 0 — Request normalization
Normalize solution path, scope filters, feature flags, output path preferences, and cache controls.
Record a deterministic `AnalysisRequest`.

## Stage 1 — Workspace loading
Use MSBuild discovery and Roslyn workspace loading.
Capture diagnostics for invalid SDKs, unsupported projects, or partial load failures.

## Stage 2 — Solution inventory
Collect solution/project/document inventory, target frameworks, package references, and top-level metadata.
This stage should already support a useful “basic overview” mode.

## Stage 3 — Symbol facts
Collect namespaces, types, members, inheritance, interfaces, source locations, and XML docs.

## Stage 4 — Specialized facts
Collect:
- dependency graphs
- DI registrations
- DbContexts/entities/relationships
- optional basic size/shape metrics

## Stage 5 — Insights
Run deterministic risk rules and graph/persistence/service analysis over normalized facts.

## Stage 6 — Snapshot assembly
Combine request, facts, insights, diagnostics, and export catalog into a full canonical snapshot.

## Stage 7 — Rendering/export
Generate Markdown summaries, Mermaid diagrams, and export metadata.

## Stage 8 — Storage/recent index
Persist the snapshot, index it in recent history, and optionally reuse cached outputs for equivalent requests.

## Stage 9 — Query and presentation
Serve the snapshot into:
- application services,
- the SSR UI,
- a future MCP driver.

## Project mapping

- `Workspace` => stages 0-2
- `Facts` => stages 3-4
- `Analysis` => stage 5
- `Storage` => stages 6 and 8
- `Rendering` => stage 7
- `Application` => orchestration across all stages
- `Web` => stage 9
