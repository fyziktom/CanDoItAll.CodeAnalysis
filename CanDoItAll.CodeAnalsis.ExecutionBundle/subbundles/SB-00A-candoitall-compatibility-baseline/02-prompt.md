# Prompt for SB-00A — Current CanDoItAll compatibility baseline

You are implementing **SB-00A** inside the repository `CanDoItAll.CodeAnalsis`.

## Goal
Translate the current CanDoItAll MCP ecosystem into explicit compatibility rules, naming decisions, and future integration seams before deep implementation begins.

## Read before coding
- overview/11-future-mcp-handoff.md
- overview/15-current-candoitall-mcp-landscape.md
- overview/16-compatibility-and-shared-parts.md
- overview/17-naming-settings-and-tool-surface-map.md
- reference/current-candoitall-mcp-context.md
- reference/current-candoitall-mcp-context.json
- subbundles/SB-00A-candoitall-compatibility-baseline/01-scope.md
- subbundles/SB-00A-candoitall-compatibility-baseline/03-checklist.md
- subbundles/SB-00A-candoitall-compatibility-baseline/04-validation.md
- subbundles/SB-00A-candoitall-compatibility-baseline/05-forbidden-patterns.md
- subbundles/SB-00A-candoitall-compatibility-baseline/06-required-evidence.md

## Also inspect these current CanDoItAll repo files
- .vscode/mcp.json
- CanDoItAll.Mcp.Components.settings.json
- CanDoItAll.Mcp.DotNetWatch.settings.json
- CanDoItAll.Mcp.ProjectStructure.settings.example.json
- src/CanDoItAll.Mcp.Components/Program.cs
- src/CanDoItAll.Mcp.ProjectStructure/Program.cs
- src/CanDoItAll.Mcp.DotNetWatch/Program.cs
- src/CanDoItAll.Mcp.Core/Contracts/McpToolEnvelope.cs
- tools/Reinstall-CanDoItAllMcps.ps1
- codex/README.md
- .codex/agents/arch-mapper.toml

## Required implementation steps
- Create a compatibility matrix that maps current host-repo MCP patterns to future code-analytics integration points.
- Record exact future tool names and their mapping to application-layer operations.
- Document how the standalone repo will eventually plug into `tools/Reinstall-CanDoItAllMcps.ps1`, `.vscode/mcp.json`, and repo-managed Codex assets.
- Decide whether the standalone repo ships any local Codex assets now or only design-ready placeholders.

## Cross-cutting guardrails
- Respect the naming map: repo/solution `CanDoItAll.CodeAnalsis`, project family `CanDoItAll.CodeAnalytics.*`, future driver `CanDoItAll.Mcp.CodeAnalytics`.
- Treat `.slnx` as canonical unless a concrete tool blocker proves otherwise.
- Do not clone `CanDoItAll.Mcp.Core` or any host-repo-only MCP runtime types into the standalone libraries.
- Keep facts, insights, and diagnostics separate.
- Keep comments in English and rare. Avoid XML docs unless a public-contract reason clearly justifies them.
- Keep files small and cohesive. If you temporarily create long files for speed, you must split them before closure.
- Avoid dumping-ground folders such as `Helpers`, `Misc`, or `Stuff`.

## Subbundle-specific stop rules
- Do not continue if the naming map is still ambiguous.
- Do not continue if future MCP integration still requires redesigning the application-layer contracts.
- Do not continue if the standalone repo starts accreting host-specific infrastructure.

## Before you call this subbundle done
- run the listed validation commands,
- update or add the required evidence artifacts,
- verify compatibility with the future `CanDoItAll.Mcp.CodeAnalytics` seam,
- do a local cleanup pass on file size and folder hygiene.
