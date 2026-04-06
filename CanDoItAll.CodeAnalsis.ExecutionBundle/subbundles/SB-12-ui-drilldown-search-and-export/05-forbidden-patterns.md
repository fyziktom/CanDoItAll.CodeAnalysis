# Forbidden patterns for SB-12 — UI drilldown search and export

- Putting every drilldown into one huge Razor file.
- Duplicating application query logic in the UI.

## Also forbidden globally
- breaking the naming map,
- leaking host-only MCP transport/runtime types into standalone libraries,
- leaving silent unsupported cases,
- skipping the final refactor/review passes.
