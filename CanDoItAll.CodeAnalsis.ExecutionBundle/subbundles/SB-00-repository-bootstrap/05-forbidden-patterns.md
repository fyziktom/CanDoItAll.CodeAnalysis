# Forbidden patterns for SB-00 — Repository bootstrap and guardrails

- Creating `Helpers`, `Misc`, or `CommonStuff` folders as catch-all bins.
- Making `.sln` canonical when `.slnx` works.
- Adding MCP envelope or host-runtime types into the standalone core libraries.
- Leaving long generated files unsplit at the end of the subbundle.

## Also forbidden globally
- breaking the naming map,
- leaking host-only MCP transport/runtime types into standalone libraries,
- leaving silent unsupported cases,
- skipping the final refactor/review passes.
