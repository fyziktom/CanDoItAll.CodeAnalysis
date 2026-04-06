# Future MCP handoff

## Goal

After the standalone repo is stable, the main CanDoItAll repo should be able to add:

- `src/CanDoItAll.Mcp.CodeAnalytics/`
- `tests/CanDoItAll.Mcp.CodeAnalytics.Tests/`
- root settings/example files
- `.vscode/mcp.json` registration
- reinstall/publish script updates
- optional repo-managed Codex skill updates

without rewriting the analysis engine.

## Thin-driver principle

The future driver should mainly do these things:

1. bind and validate MCP server settings,
2. resolve host-repo identities, correlation IDs, and logging,
3. map MCP tool inputs to application requests,
4. wrap results into `McpToolEnvelope<T>`,
5. expose structured diagnostics and possibly operation status,
6. register in install scripts and VS Code MCP config.

## What must already be true in the standalone repo

- no host-only infrastructure types are required by the core libraries,
- the application layer already exposes the operations the tools need,
- storage/export logic is reusable,
- contracts are transport-agnostic,
- naming/settings/tool surface are already frozen.

## Host-repo touch points during future integration

At minimum expect to update:
- `src/CanDoItAll.Mcp.CodeAnalytics/*` (new)
- `tests/CanDoItAll.Mcp.CodeAnalytics.Tests/*` (new)
- `.vscode/mcp.json`
- a root settings file such as `CanDoItAll.Mcp.CodeAnalytics.settings.example.json`
- `tools/Reinstall-CanDoItAllMcps.ps1`
- `codex/README.md` and possibly `codex/skills/*`

## Recommended future MCP tools

- `code_analytics_snapshot_build`
- `code_analytics_summary_get`
- `code_analytics_diagram_get`
- `code_analytics_query`
- `code_analytics_export_get`
- `code_analytics_recent_list`
