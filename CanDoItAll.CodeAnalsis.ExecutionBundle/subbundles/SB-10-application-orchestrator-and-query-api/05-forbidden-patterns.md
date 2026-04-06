# Forbidden patterns for SB-10 — Application orchestrator and query API

- Using static service locators or hidden singletons in the application layer.
- Returning renderer-specific strings where structured data is expected.

## Also forbidden globally
- breaking the naming map,
- leaking host-only MCP transport/runtime types into standalone libraries,
- leaving silent unsupported cases,
- skipping the final refactor/review passes.
