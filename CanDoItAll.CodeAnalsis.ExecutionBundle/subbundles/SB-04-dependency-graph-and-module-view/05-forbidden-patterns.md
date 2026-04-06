# Forbidden patterns for SB-04 — Dependency graph and module view

- Embedding Mermaid text generation into the dependency collector itself.
- Using mutable global graph state that is hard to test.
- Hardcoding module names for one fixture instead of deriving them.

## Also forbidden globally
- breaking the naming map,
- leaking host-only MCP transport/runtime types into standalone libraries,
- leaving silent unsupported cases,
- skipping the final refactor/review passes.
