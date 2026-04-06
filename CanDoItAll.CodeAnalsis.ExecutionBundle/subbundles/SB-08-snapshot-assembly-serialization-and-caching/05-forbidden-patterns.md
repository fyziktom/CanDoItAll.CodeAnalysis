# Forbidden patterns for SB-08 — Snapshot assembly, serialization, and caching

- Storing only rendered markdown/diagram outputs and not the canonical snapshot.
- Leaking UI or MCP envelope types into storage contracts.

## Also forbidden globally
- breaking the naming map,
- leaking host-only MCP transport/runtime types into standalone libraries,
- leaving silent unsupported cases,
- skipping the final refactor/review passes.
