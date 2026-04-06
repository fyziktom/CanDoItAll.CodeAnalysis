# Prompt for SB-13 — Future CanDoItAll MCP driver seam and compatibility proof

You are implementing **SB-13** inside the repository `CanDoItAll.CodeAnalsis`.

## Goal
Prove that the standalone repo is ready to become a future `CanDoItAll.Mcp.CodeAnalytics` driver in the main CanDoItAll repo without architectural rewrites.

## Read before coding
- overview/11-future-mcp-handoff.md
- overview/15-current-candoitall-mcp-landscape.md
- overview/16-compatibility-and-shared-parts.md
- overview/17-naming-settings-and-tool-surface-map.md
- reference/CanDoItAll.Mcp.CodeAnalytics.settings.example.json
- reference/vscode-mcp-snippet.code-analytics.json
- reference/tool-surface-proposal.json
- subbundles/SB-13-future-candoitall-mcp-driver-seam/01-scope.md
- subbundles/SB-13-future-candoitall-mcp-driver-seam/03-checklist.md
- subbundles/SB-13-future-candoitall-mcp-driver-seam/04-validation.md
- subbundles/SB-13-future-candoitall-mcp-driver-seam/05-forbidden-patterns.md
- subbundles/SB-13-future-candoitall-mcp-driver-seam/06-required-evidence.md

## Also inspect these current CanDoItAll repo files
- src/CanDoItAll.Mcp.Components/Program.cs
- src/CanDoItAll.Mcp.Components/Tools/ComponentsTools.cs
- src/CanDoItAll.Mcp.ProjectStructure/Program.cs
- src/CanDoItAll.Mcp.ProjectStructure/ProjectStructureTools.cs
- src/CanDoItAll.Mcp.DotNetWatch/Program.cs
- .vscode/mcp.json
- tools/Reinstall-CanDoItAllMcps.ps1

## Required implementation steps
- Write a driver-shape doc showing `Program`, `Configuration`, `Tools`, and coordinator/service seams.
- Define exact tool names such as `code_analytics_snapshot_build`, `code_analytics_summary_get`, `code_analytics_diagram_get`, `code_analytics_query`, `code_analytics_export_get`, and `code_analytics_recent_list`.
- Prove application contracts are sufficient for these tools without refactoring.
- List the exact host-repo files that will need updates during future transplantation.

## Cross-cutting guardrails
- Respect the naming map: repo/solution `CanDoItAll.CodeAnalsis`, project family `CanDoItAll.CodeAnalytics.*`, future driver `CanDoItAll.Mcp.CodeAnalytics`.
- Treat `.slnx` as canonical unless a concrete tool blocker proves otherwise.
- Do not clone `CanDoItAll.Mcp.Core` or any host-repo-only MCP runtime types into the standalone libraries.
- Keep facts, insights, and diagnostics separate.
- Keep comments in English and rare. Avoid XML docs unless a public-contract reason clearly justifies them.
- Keep files small and cohesive. If you temporarily create long files for speed, you must split them before closure.
- Avoid dumping-ground folders such as `Helpers`, `Misc`, or `Stuff`.

## Subbundle-specific stop rules
- Do not continue if future tool names/settings are still fluid.
- Do not continue if the future driver would still need to redesign the application API.

## Before you call this subbundle done
- run the listed validation commands,
- update or add the required evidence artifacts,
- verify compatibility with the future `CanDoItAll.Mcp.CodeAnalytics` seam,
- do a local cleanup pass on file size and folder hygiene.
