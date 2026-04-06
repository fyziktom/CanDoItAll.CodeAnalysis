# Compatibility and shared parts

## Reuse later, do not duplicate now

The standalone repo should **not** re-implement these host-repo responsibilities:

- `McpToolEnvelope<T>` and tool-error envelopes
- correlation/server identity factories from `CanDoItAll.Mcp.Core`
- host-repo MCP bootstrapping boilerplate
- install/publish/reinstall scripts
- workspace-wide VS Code MCP registration

These belong to the future host-repo driver layer.

## Build now in a host-friendly way

The standalone repo **should** build these reusable seams now:

- transport-agnostic application service contracts,
- canonical snapshot model,
- deterministic export catalog,
- query API shaped around future MCP tools,
- diagnostics that map cleanly into structured tool responses later.

## Shared-part mapping

### Standalone repo responsibility
- analysis engine
- canonical snapshot
- summaries and Mermaid
- storage and recent index
- SSR UI

### Future host-repo responsibility
- `CanDoItAll.Mcp.CodeAnalytics`
- wrapping results into `McpToolEnvelope<T>`
- settings validation and server runtime
- `.vscode/mcp.json` registration
- install/publish/reinstall script integration
- optional workflow guidance or operation tracking

## Naming compatibility rules

- root repo + canonical solution => `CanDoItAll.CodeAnalsis`
- project/assembly/namespace family => `CanDoItAll.CodeAnalytics.*`
- future host-repo driver => `CanDoItAll.Mcp.CodeAnalytics`
- future tool prefix => `code_analytics_`

## Why this split matters

If the standalone repo clones host-repo-only MCP concerns too early, transplantation becomes harder, not easier.
If it stays purely engine-oriented with a thin-driver seam, future integration is mostly wiring and testing.
