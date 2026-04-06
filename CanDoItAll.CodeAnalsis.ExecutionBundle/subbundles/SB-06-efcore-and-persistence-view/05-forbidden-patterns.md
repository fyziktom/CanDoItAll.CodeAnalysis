# Forbidden patterns for SB-06 — EF Core and persistence view

- Making diagram generation the only representation of persistence information.
- Assuming all EF models are trivial or fully resolvable statically.

## Also forbidden globally
- breaking the naming map,
- leaking host-only MCP transport/runtime types into standalone libraries,
- leaving silent unsupported cases,
- skipping the final refactor/review passes.
