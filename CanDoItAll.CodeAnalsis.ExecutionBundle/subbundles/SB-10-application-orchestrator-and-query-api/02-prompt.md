# Prompt for SB-10 — Application orchestrator and query API

You are implementing **SB-10** inside the repository `CanDoItAll.CodeAnalsis`.

## Goal
Expose stable application-layer operations that build snapshots, fetch summaries, enumerate exports, and answer focused questions without coupling callers to Roslyn internals.

## Read before coding
- overview/01-executive-summary.md
- overview/11-future-mcp-handoff.md
- overview/17-naming-settings-and-tool-surface-map.md
- subbundles/SB-10-application-orchestrator-and-query-api/01-scope.md
- subbundles/SB-10-application-orchestrator-and-query-api/03-checklist.md
- subbundles/SB-10-application-orchestrator-and-query-api/04-validation.md
- subbundles/SB-10-application-orchestrator-and-query-api/05-forbidden-patterns.md
- subbundles/SB-10-application-orchestrator-and-query-api/06-required-evidence.md

## Also inspect these current CanDoItAll repo files
- src/CanDoItAll.Mcp.ProjectStructure/ProjectStructureCoordinator.cs
- src/CanDoItAll.Mcp.Components/Tools/ComponentsTools.cs
- src/CanDoItAll.Mcp.Core/Contracts/McpToolEnvelope.cs

## Required implementation steps
- Implement application entrypoints such as `BuildSnapshot`, `GetSummary`, `GetDiagram`, `ListRecentSnapshots`, and `QuerySnapshot`.
- Normalize failure categories and diagnostic propagation.
- Ensure cancellation tokens flow across expensive stages.
- Add orchestration tests using fixture repositories or fakes.

## Cross-cutting guardrails
- Respect the naming map: repo/solution `CanDoItAll.CodeAnalsis`, project family `CanDoItAll.CodeAnalytics.*`, future driver `CanDoItAll.Mcp.CodeAnalytics`.
- Treat `.slnx` as canonical unless a concrete tool blocker proves otherwise.
- Do not clone `CanDoItAll.Mcp.Core` or any host-repo-only MCP runtime types into the standalone libraries.
- Keep facts, insights, and diagnostics separate.
- Keep comments in English and rare. Avoid XML docs unless a public-contract reason clearly justifies them.
- Keep files small and cohesive. If you temporarily create long files for speed, you must split them before closure.
- Avoid dumping-ground folders such as `Helpers`, `Misc`, or `Stuff`.

## Subbundle-specific stop rules
- Do not continue if application services leak Roslyn types or UI models.
- Do not continue if error categories are too vague for future MCP wrapping.

## Before you call this subbundle done
- run the listed validation commands,
- update or add the required evidence artifacts,
- verify compatibility with the future `CanDoItAll.Mcp.CodeAnalytics` seam,
- do a local cleanup pass on file size and folder hygiene.
