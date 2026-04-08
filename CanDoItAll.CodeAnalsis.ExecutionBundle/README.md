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

It is reopened again on 2026-04-07 after the direct SharpTools comparison exposed one correctness defect and two tuning defects in the new focused-context feature:

- shared-helper searches can fail on duplicate normalized source paths from package-backed generated files,
- broad type and helper seeds still over-expand because constructor-heavy and type-only fallbacks are too generous,
- the tuning UI needs clearer quality cues so noisy selections are easier to judge quickly.

It is reopened again on 2026-04-08 after the helper-noise analysis showed that high-fan-in helpers such as `IClock` still need a more surgical mode:

- the current traversal still treats ubiquitous helpers like broken workflow methods,
- helper exploration needs definitions, implementations, and sampled usages instead of undirected consumer spread,
- the next pass must land the minimal helper-precision change set first, then the maintainability refactor, then the broader helper-mode improvements.

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
- [04-sharptools-comparison-follow-up.md](C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.ExecutionBundle\inputs\04-sharptools-comparison-follow-up.md)
- [05-helper-surgical-precision-follow-up.md](C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.ExecutionBundle\inputs\05-helper-surgical-precision-follow-up.md)
- [01-current-state.md](C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.ExecutionBundle\analysis\01-current-state.md)
- [01-target-solution.md](C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.ExecutionBundle\architecture\01-target-solution.md)
- [01-phase-plan.md](C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.ExecutionBundle\plan\01-phase-plan.md)
- [01-requirement-traceability.md](C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.ExecutionBundle\traceability\01-requirement-traceability.md)
- [CanDoItAll.CodeAnalsis.ExecutionBacklog.xlsx](C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.ExecutionBundle\spreadsheets\CanDoItAll.CodeAnalsis.ExecutionBacklog.xlsx)

## Validation Summary

- Bundle preparation status: `Completed and reopened helper scope is now implemented on 2026-04-08`
- Bundle readiness gate: `Passed`
- Execution status: `Completed`
- Subbundle gate review: `SB-00 through SB-19 remain trusted; SB-20 through SB-23 passed`
- Final closure gate: `Passed`
- Browser validation analytics: `Helper auto mode, helper usage-summary mode, database preserve, and UI preserve were rerun on 2026-04-08`

## Current Focus

- The comparison-driven repair cycle remains the trusted baseline.
- The helper-precision reopen is closed.
- High-fan-in helpers now route through typed intent and precision controls, targeted strategy ownership, implementation-aware shaping, and clustered usage summaries.
- The remaining residual issue is operational rather than feature correctness: the managed watch health probe stayed flaky during browser proof even though the page itself rendered correctly at `http://127.0.0.1:5501`.

## Notes

- The original bundle artifacts are kept in place and are still referenced where they remain valid.
- The bundle is now maintained as an `initiative` profile bundle with inventories, templates, and traceability.
- Execution must not change anything under `C:\repositories\CanDoItAll`.
- The standalone engine still preserves a thin future `CanDoItAll.Mcp.CodeAnalytics` seam. This reopen remains a heuristic and maintainability pass for high-reuse helper symbols, not a transport redesign.
