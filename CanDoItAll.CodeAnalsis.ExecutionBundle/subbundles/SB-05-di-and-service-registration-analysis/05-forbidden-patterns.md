# Forbidden patterns for SB-05 — DI and service registration analysis

- Scanning only string literals or text matches without semantic checks.
- Pretending to resolve every arbitrary factory lambda with certainty.

## Also forbidden globally
- breaking the naming map,
- leaking host-only MCP transport/runtime types into standalone libraries,
- leaving silent unsupported cases,
- skipping the final refactor/review passes.
