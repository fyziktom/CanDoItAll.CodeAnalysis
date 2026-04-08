# CanDoItAll.CodeAnalsis bundle

This bundle was reopened on 2026-04-07 because the original implementation reached functional baseline but did not yet deliver the stronger architecture navigation outcomes now required:

- refactor-first hardening of oversized ownership hotspots,
- project-scoped and context-scoped diagram usefulness,
- stronger EF schema relationship recovery,
- member-level trouble-path context queries that save agent context,
- a bundle structure that is executable under the current CanDoItAll workflow contract.

It was reopened again on 2026-04-07 after a newer focused-context request exposed four important gaps in the shipped surface:

- free-text entry from exception, compile-error, or developer prompt text,
- tag-driven focus so the traversal can bias toward database, UI, or similar intent,
- per-file code excerpts with line-count stats instead of source links alone,
- a dedicated tuning page where solution or project scope, prompt, tags, and resulting excerpts can be judged together.

The naming map remains frozen:

- Repo root: `CanDoItAll.CodeAnalsis`
- Canonical solution: `CanDoItAll.CodeAnalsis.slnx`
- Project and namespace family: `CanDoItAll.CodeAnalytics.*`
- Future MCP host driver: `CanDoItAll.Mcp.CodeAnalytics`

## Bundle Scope

- Preserve the standalone repo as the canonical engine.
- Keep the future MCP seam thin.
- Refactor before widening features.
- Add context-focused analysis outputs that reduce agent call count and token waste versus raw file loading.
- Keep the host CanDoItAll repository read-only and use it only as validation and compatibility reference.

## Key Inputs

- [00-original-request.md](C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.ExecutionBundle\inputs\00-original-request.md)
- [01-source-artifacts.md](C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.ExecutionBundle\inputs\01-source-artifacts.md)
- [02-structured-input.md](C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.ExecutionBundle\inputs\02-structured-input.md)
- [03-focused-context-lab-request.md](C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.ExecutionBundle\inputs\03-focused-context-lab-request.md)
- [01-current-state.md](C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.ExecutionBundle\analysis\01-current-state.md)
- [01-target-solution.md](C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.ExecutionBundle\architecture\01-target-solution.md)
- [01-phase-plan.md](C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.ExecutionBundle\plan\01-phase-plan.md)
- [01-requirement-traceability.md](C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.ExecutionBundle\traceability\01-requirement-traceability.md)
- [CanDoItAll.CodeAnalsis.ExecutionBacklog.xlsx](C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.ExecutionBundle\spreadsheets\CanDoItAll.CodeAnalsis.ExecutionBacklog.xlsx)

## Validation Summary

- Bundle preparation status: `Repaired and validated on 2026-04-07`
- Bundle readiness gate: `Passed`
- Execution status: `Completed on 2026-04-07`
- Subbundle gate review: `SB-15 through SB-19 passed`
- Final closure gate: `Passed`
- Browser validation analytics: `Fixture and host focused-context lab flows captured`

## Current Focus

- Completed baseline subbundles remain the foundation for repo bootstrap, compatibility, Roslyn loading, snapshot storage, and the first UI shell.
- `SB-15` and `SB-16` remain trusted and no regressions were introduced in the reopened pass.
- `SB-17` now closes with text-driven seed resolution, focus tags, grouped excerpts, and stats.
- `SB-18` now closes with the dedicated focused-context lab page and accordion-based review flow.
- `SB-19` now closes with final validation, host sanity proof, SharpTools comparison, and explicit tuning guidance for the next heuristic pass.

## Notes

- The original bundle artifacts are kept in place and are still referenced where they remain valid.
- The bundle is now maintained as an `initiative` profile bundle with inventories, templates, and traceability.
- Execution must not change anything under `C:\repositories\CanDoItAll`.
- The standalone engine still preserves a thin future `CanDoItAll.Mcp.CodeAnalytics` seam. The remaining follow-up work is heuristic tuning, not transport redesign.
