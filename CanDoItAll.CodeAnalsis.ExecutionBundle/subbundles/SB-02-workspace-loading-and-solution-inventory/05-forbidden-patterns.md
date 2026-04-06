# Forbidden patterns for SB-02 — Workspace loading and solution inventory

- Falling back to naive file scanning as the default inventory path.
- Skipping diagnostics when project load fails.
- Returning unstable ordering due to direct dictionary iteration.

## Also forbidden globally
- breaking the naming map,
- leaking host-only MCP transport/runtime types into standalone libraries,
- leaving silent unsupported cases,
- skipping the final refactor/review passes.
