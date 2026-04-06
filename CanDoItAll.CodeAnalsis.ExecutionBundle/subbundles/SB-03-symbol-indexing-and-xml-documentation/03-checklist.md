# Checklist for SB-03 — Symbol indexing and XML documentation ingestion

## Build and structure
- [ ] Relevant projects/folders exist and are correctly named.
- [ ] Project references still follow the intended architecture.
- [ ] `.slnx` remains canonical unless a concrete blocker forced a justified compatibility file.
- [ ] No host-repo MCP core clone was introduced.
- [ ] No dumping-ground folders were introduced.
- [ ] Oversized files were split or explicitly justified.

## Functional work
- [ ] Type/member fact collectors.
- [ ] XML documentation resolver and normalizer.
- [ ] Unit/integration tests around generics, partial types, and missing docs.

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
