# Forbidden patterns for SB-14 — Tests hardening, repo-local Codex assets, and final refactor

- Leaving giant files in place after the final pass.
- Treating the review pass as optional.
- Adding Codex assets that contradict the bundle or the host-repo style.

## Also forbidden globally
- breaking the naming map,
- leaking host-only MCP transport/runtime types into standalone libraries,
- leaving silent unsupported cases,
- skipping the final refactor/review passes.
