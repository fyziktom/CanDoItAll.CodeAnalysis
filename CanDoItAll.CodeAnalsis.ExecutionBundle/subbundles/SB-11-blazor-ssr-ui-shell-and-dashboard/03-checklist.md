# Checklist for SB-11 — Blazor SSR UI shell and dashboard

## Build and structure
- [ ] Relevant projects/folders exist and are correctly named.
- [ ] Project references still follow the intended architecture.
- [ ] `.slnx` remains canonical unless a concrete blocker forced a justified compatibility file.
- [ ] No host-repo MCP core clone was introduced.
- [ ] No dumping-ground folders were introduced.
- [ ] Oversized files were split or explicitly justified.

## Functional work
- [ ] SSR app shell and core routes.
- [ ] Dashboard cards/tables for recent snapshots and top findings.
- [ ] Web tests for main SSR flows.

## Current CanDoItAll compatibility
- [ ] The naming map is still respected.
- [ ] The work remains compatible with a future thin `CanDoItAll.Mcp.CodeAnalytics` driver.
- [ ] Any host-repo pattern adopted here was adopted intentionally, not copied blindly.
- [ ] Any host-repo-only concern remains outside the standalone libraries.

## Quality and maintainability
- [ ] Diagnostics are explicit and actionable.
- [ ] Deterministic ordering is preserved.
- [ ] Cancellation flows where relevant.
- [ ] Tests cover fragile or ambiguity-prone behavior.
- [ ] Comments remain English-only and sparse.
