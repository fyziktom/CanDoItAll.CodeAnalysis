# SB-00A — Current CanDoItAll compatibility baseline

## Objective
Translate the current CanDoItAll MCP ecosystem into explicit compatibility rules, naming decisions, and future integration seams before deep implementation begins.

## Milestone / priority / actor
- Milestone: `M0`
- Priority: `P0`
- Primary actor: `Integration architect`

## Depends on
- SB-00

## Read first
- overview/11-future-mcp-handoff.md
- overview/15-current-candoitall-mcp-landscape.md
- overview/16-compatibility-and-shared-parts.md
- overview/17-naming-settings-and-tool-surface-map.md
- overview/19-host-repo-shared-surface-catalog.md
- reference/current-candoitall-mcp-context.md
- reference/current-candoitall-mcp-context.json

## Current CanDoItAll reference files to inspect
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

## In scope
- Capture the current host-repo MCP patterns in project docs so Codex does not drift into an incompatible standalone architecture.
- Lock the naming map between repo typo (`CodeAnalsis`) and namespace family (`CodeAnalytics`).
- Define the future MCP driver surface, tool prefixes, settings file names, and expected `.vscode/mcp.json` registration style.
- Document which shared parts must be reused later from `CanDoItAll.Mcp.Core` instead of duplicated now.

## Out of scope
- No runtime MCP server implementation yet.
- No publishing or install scripts yet beyond design-level placeholders.
- No host-repo code changes.

## Compatibility rules specific to this subbundle
- Future `CanDoItAll.Mcp.CodeAnalytics` must follow the host-repo shape: `Program`, `Configuration`, `Tools`, and coordinator/runtime folders as appropriate.
- The current host repo uses `Host.CreateEmptyApplicationBuilder(settings: null)`, JSON settings + `CanDoItAllMcp_` env prefix, explicit options validation, and `AddMcpServer().WithStdioServerTransport().WithTools<...>()`.
- Tool names should use lowercase snake_case with a `code_analytics_` prefix to match the style of current CanDoItAll servers.

## Expected deliverables
- Compatibility decision docs committed into the repo.
- Future settings file example and `.vscode/mcp.json` snippet added to `reference/`.
- Explicit list of “reuse later” vs “do not duplicate now” host-repo shared parts.
