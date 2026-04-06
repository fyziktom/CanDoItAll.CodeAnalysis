# Checklist for SB-00A — Current CanDoItAll compatibility baseline

## Build and structure
- [ ] Relevant projects/folders exist and are correctly named.
- [ ] Project references still follow the intended architecture.
- [ ] `.slnx` remains canonical unless a concrete blocker forced a justified compatibility file.
- [ ] No host-repo MCP core clone was introduced.
- [ ] No dumping-ground folders were introduced.
- [ ] Oversized files were split or explicitly justified.

## Functional work
- [ ] Compatibility decision docs committed into the repo.
- [ ] Future settings file example and `.vscode/mcp.json` snippet added to `reference/`.
- [ ] Explicit list of “reuse later” vs “do not duplicate now” host-repo shared parts.

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
