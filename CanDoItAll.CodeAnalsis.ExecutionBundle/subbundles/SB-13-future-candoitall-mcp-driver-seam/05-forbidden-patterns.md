# Forbidden patterns for SB-13 — Future CanDoItAll MCP driver seam and compatibility proof

- Adding a premature standalone MCP project just to “prove” compatibility.
- Duplicating host-repo bootstrapping helpers inside this repo.

## Also forbidden globally
- breaking the naming map,
- leaking host-only MCP transport/runtime types into standalone libraries,
- leaving silent unsupported cases,
- skipping the final refactor/review passes.
