# Forbidden patterns for SB-09 — Summary writers and Mermaid renderers

- Hardcoding diagrams directly from Roslyn or EF APIs instead of canonical snapshot records.
- Generating one monolithic “everything” diagram by default.

## Also forbidden globally
- breaking the naming map,
- leaking host-only MCP transport/runtime types into standalone libraries,
- leaving silent unsupported cases,
- skipping the final refactor/review passes.
