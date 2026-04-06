# ADR-006 — align the future driver with current CanDoItAll MCP patterns

## Status
Accepted

## Decision

The future `CanDoItAll.Mcp.CodeAnalytics` project should follow the host-repo MCP style:
- `Program`
- `Configuration`
- `Tools`
- coordinator/runtime folders as needed
- settings file + `.vscode/mcp.json` registration
- artifact-backed install integration later

## Rationale

This minimizes conceptual drift and reduces future transplant friction.
