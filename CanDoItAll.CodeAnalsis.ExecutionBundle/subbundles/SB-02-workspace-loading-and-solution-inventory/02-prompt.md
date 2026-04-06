# Prompt for SB-02 — Workspace loading and solution inventory

You are implementing **SB-02** inside the repository `CanDoItAll.CodeAnalsis`.

## Goal
Load .NET solutions through Roslyn/MSBuild, normalize requests, and produce a deterministic inventory of solutions, projects, and documents.

## Read before coding
- overview/05-analysis-pipeline.md
- adrs/ADR-002-roslyn-first-analysis.md
- overview/17-naming-settings-and-tool-surface-map.md
- subbundles/SB-02-workspace-loading-and-solution-inventory/01-scope.md
- subbundles/SB-02-workspace-loading-and-solution-inventory/03-checklist.md
- subbundles/SB-02-workspace-loading-and-solution-inventory/04-validation.md
- subbundles/SB-02-workspace-loading-and-solution-inventory/05-forbidden-patterns.md
- subbundles/SB-02-workspace-loading-and-solution-inventory/06-required-evidence.md

## Also inspect these current CanDoItAll repo files
- global.json
- Directory.Build.props
- CanDoItAll.slnx
- .github/copilot-instructions.md

## Required implementation steps
- Wire `MSBuildLocator` and `MSBuildWorkspace` or an equivalent safe loading pipeline.
- Map Roslyn workspace data into domain-friendly inventory records immediately.
- Emit actionable diagnostics for missing SDKs, invalid paths, unsupported projects, or load failures.
- Cache or memoize inexpensive inventory data where it reduces duplicate workspace work.

## Cross-cutting guardrails
- Respect the naming map: repo/solution `CanDoItAll.CodeAnalsis`, project family `CanDoItAll.CodeAnalytics.*`, future driver `CanDoItAll.Mcp.CodeAnalytics`.
- Treat `.slnx` as canonical unless a concrete tool blocker proves otherwise.
- Do not clone `CanDoItAll.Mcp.Core` or any host-repo-only MCP runtime types into the standalone libraries.
- Keep facts, insights, and diagnostics separate.
- Keep comments in English and rare. Avoid XML docs unless a public-contract reason clearly justifies them.
- Keep files small and cohesive. If you temporarily create long files for speed, you must split them before closure.
- Avoid dumping-ground folders such as `Helpers`, `Misc`, or `Stuff`.

## Subbundle-specific stop rules
- Do not continue if workspace loading still leaks Roslyn types into the application layer.
- Do not continue if invalid paths produce silent or cryptic failures.

## Before you call this subbundle done
- run the listed validation commands,
- update or add the required evidence artifacts,
- verify compatibility with the future `CanDoItAll.Mcp.CodeAnalytics` seam,
- do a local cleanup pass on file size and folder hygiene.
