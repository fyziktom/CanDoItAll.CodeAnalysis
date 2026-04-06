# Forbidden patterns for SB-03 — Symbol indexing and XML documentation ingestion

- Stuffing raw Roslyn symbol display strings everywhere without normalized contracts.
- Assuming XML docs always exist.
- Mixing symbol facts with inferred architectural roles.

## Also forbidden globally
- breaking the naming map,
- leaking host-only MCP transport/runtime types into standalone libraries,
- leaving silent unsupported cases,
- skipping the final refactor/review passes.
