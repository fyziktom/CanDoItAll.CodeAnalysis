# ADR-007 — do not duplicate `CanDoItAll.Mcp.Core`

## Status
Accepted

## Decision

The standalone repo must not copy `McpToolEnvelope`, correlation factories, operation primitives, or other host MCP core helpers.

## Rationale

Those are transport/runtime concerns of the host repo.
The standalone repo should stay engine-first and transport-agnostic.
