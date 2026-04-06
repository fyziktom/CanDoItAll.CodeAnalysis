# Compatibility Matrix

| Host-repo surface | Current host pattern | Standalone rule now | Future `CanDoItAll.Mcp.CodeAnalytics` action |
|---|---|---|---|
| Solution and SDK | `CanDoItAll.slnx`, `10.0.200`, `latestPatch`, `net10.0` | Keep `.slnx` canonical and pin `10.0.200` with `latestPatch` in `global.json`. | Add the future driver project to the host `.slnx` without changing the engine projects. |
| MCP bootstrap | `Program` + configuration/options + services/coordinator + `Tools` class | Keep the standalone repo transport-agnostic. Do not add MCP server runtime code here. | Implement the thin `Program`, `Configuration`, `Tools`, and coordinator wrappers in the host repo. |
| Tool envelopes and failures | `McpToolEnvelope<T>`, `ToolError`, `ToolInvocationException` in `CanDoItAll.Mcp.Core` | Do not copy host envelope or exception types into standalone libraries. | Reuse `CanDoItAll.Mcp.Core` directly when the driver wraps application results. |
| Settings and env mapping | JSON settings files plus `CanDoItAllMcp_` environment prefix | Freeze the future settings shape in `reference/` only. Do not add a real MCP settings loader yet. | Bind and validate `CanDoItAll.Mcp.CodeAnalytics.settings*.json` in the host driver. |
| VS Code MCP registration | `.vscode/mcp.json` uses stdio entries, artifact-backed executables, and `--settings` | Keep only a design-ready snippet in `reference/`. Do not register a local MCP server from the standalone repo. | Add a `candoitall_codeanalytics` entry in the host repo when the driver exists. |
| Install and publish flow | `tools/Reinstall-CanDoItAllMcps.ps1` publishes installs and updates Codex/VS Code config | Document the touch points only. Do not add local publish or reinstall scripts yet. | Extend `tools/Reinstall-CanDoItAllMcps.ps1` to publish and register `CanDoItAll.Mcp.CodeAnalytics`. |
| Repo-managed Codex assets | `codex/README.md`, skill pack, optional `.codex/agents` | Ship only lightweight placeholders now. Avoid host-specific skill/runtime assumptions. | Add repo-managed skills or agents only if the host integration needs them. |
