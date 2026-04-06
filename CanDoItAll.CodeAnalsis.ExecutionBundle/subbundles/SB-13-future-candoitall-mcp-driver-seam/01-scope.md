# SB-13 — Future CanDoItAll MCP driver seam and compatibility proof

## Objective
Prove that the standalone repo is ready to become a future `CanDoItAll.Mcp.CodeAnalytics` driver in the main CanDoItAll repo without architectural rewrites.

## Milestone / priority / actor
- Milestone: `M5`
- Priority: `P0`
- Primary actor: `Integration architect`

## Depends on
- SB-10
- SB-11
- SB-12

## Read first
- overview/11-future-mcp-handoff.md
- overview/15-current-candoitall-mcp-landscape.md
- overview/16-compatibility-and-shared-parts.md
- overview/17-naming-settings-and-tool-surface-map.md
- overview/19-host-repo-shared-surface-catalog.md
- reference/CanDoItAll.Mcp.CodeAnalytics.settings.example.json
- reference/vscode-mcp-snippet.code-analytics.json
- reference/tool-surface-proposal.json

## Current CanDoItAll reference files to inspect
- src/CanDoItAll.Mcp.Components/Program.cs
- src/CanDoItAll.Mcp.Components/Tools/ComponentsTools.cs
- src/CanDoItAll.Mcp.ProjectStructure/Program.cs
- src/CanDoItAll.Mcp.ProjectStructure/ProjectStructureTools.cs
- src/CanDoItAll.Mcp.DotNetWatch/Program.cs
- .vscode/mcp.json
- tools/Reinstall-CanDoItAllMcps.ps1

## In scope
- Add explicit proof artifacts that map standalone application services to future MCP tool names, settings, and install/config flows.
- Protect the future driver seam with architecture tests or contract tests.
- Document the future host-repo project/folder layout for `CanDoItAll.Mcp.CodeAnalytics`.

## Out of scope
- Do not implement the actual MCP driver project yet.
- Do not modify the host CanDoItAll repo.

## Compatibility rules specific to this subbundle
- The future driver should require only host-repo-specific bootstrapping, `McpToolEnvelope` wrapping, settings validation, and registration in `.vscode/mcp.json` / reinstall scripts.
- The standalone repo must not require its own clone of host-runtime services to prove this seam.

## Expected deliverables
- Tool-surface proposal and mapping doc.
- Future settings example and `.vscode/mcp.json` snippet.
- Architecture or contract tests proving driver readiness.
