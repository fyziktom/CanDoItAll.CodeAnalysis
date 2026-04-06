# SB-04 — Dependency graph and module view

## Objective
Derive project, assembly, namespace, and module dependency views with cycles, fan-in/fan-out, and useful module-level grouping.

## Milestone / priority / actor
- Milestone: `M2`
- Priority: `P0`
- Primary actor: `Analysis maintainer`

## Depends on
- SB-01
- SB-02
- SB-03

## Read first
- overview/02-architecture-blueprint.md
- overview/05-analysis-pipeline.md
- overview/17-naming-settings-and-tool-surface-map.md

## Current CanDoItAll reference files to inspect
- src/CanDoItAll.Composition/CanDoItAll.Composition.csproj
- src/CanDoItAll.Web/CanDoItAll.Web.csproj

## In scope
- Build project dependency and namespace dependency graphs.
- Introduce a module grouping strategy with deterministic IDs and names.
- Compute cycles, strongly connected components, fan-in, fan-out, and instability-style metrics.
- Emit a module view that summary writers and the UI can consume later.

## Out of scope
- No DI registration extraction yet.
- No persistence-specific relationships yet.

## Compatibility rules specific to this subbundle
- The module view should be useful for future analysis of the current CanDoItAll repo composition and module structure.
- Do not hardcode module rules specific to the standalone repo only; keep them configurable enough for later host-repo use.

## Expected deliverables
- Dependency graph models and collectors.
- Graph algorithms for cycle and metric computation.
- Project/namespace/module golden files and Mermaid examples.
