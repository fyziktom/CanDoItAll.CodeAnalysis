# SB-02 — Workspace loading and solution inventory

## Objective
Load .NET solutions through Roslyn/MSBuild, normalize requests, and produce a deterministic inventory of solutions, projects, and documents.

## Milestone / priority / actor
- Milestone: `M1`
- Priority: `P0`
- Primary actor: `Workspace maintainer`

## Depends on
- SB-00
- SB-00A
- SB-01

## Read first
- overview/05-analysis-pipeline.md
- adrs/ADR-002-roslyn-first-analysis.md
- overview/17-naming-settings-and-tool-surface-map.md

## Current CanDoItAll reference files to inspect
- global.json
- Directory.Build.props
- CanDoItAll.slnx
- .github/copilot-instructions.md

## In scope
- Normalize incoming solution paths and scope filters.
- Bootstrap MSBuild discovery safely and open solutions through Roslyn workspaces.
- Collect deterministic solution/project/document inventory and coarse metadata such as target frameworks and references.
- Capture loader diagnostics, timings, and partial failures.

## Out of scope
- No deep symbol analysis yet.
- No future MCP server wiring yet.
- No reflection-only fallback as the primary path.

## Compatibility rules specific to this subbundle
- Treat `.slnx` as the default shape because the current CanDoItAll repo uses it.
- Preserve enough metadata to let a future host-repo MCP driver report useful diagnostics and progress.

## Expected deliverables
- Workspace loader service and request normalization helpers.
- Solution/project/document inventory collector.
- Fixture-solution integration tests for happy and unhappy paths.
