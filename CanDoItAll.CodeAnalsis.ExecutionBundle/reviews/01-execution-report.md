# Execution report

## Status

- The bundle is reopened again on 2026-04-07 for comparison-driven repairs after the direct SharpTools analysis exposed one correctness defect and two tuning defects.
- The comparison-driven repair cycle completed on 2026-04-08 with updated build, test, browser, and SharpTools evidence.
- Prior evidence from the earlier reopened pass is retained only as the baseline for before-versus-after comparison.

## Subbundle Gate Results

| Subbundle | Entry gate | Closure gate | Downstream dependencies checked | Progression result | Notes |
| --- | --- | --- | --- | --- | --- |
| SB-15-refactor-foundation-and-canonical-ownership | Passed | Passed | Passed | Passed | Focused-context ownership is now split more clearly across seed resolution, member expansion, excerpt assembly, and lab quality helpers |
| SB-16-scoped-diagrams-and-persistence-recovery | Passed | Passed | Passed | Passed | Trusted baseline from the earlier reopened pass |
| SB-17-member-context-graph-and-query-api | Passed | Passed | Passed | Passed | Duplicate normalized-path crashes are covered, exact-type gating is tighter, and whole-type spill was replaced by representative excerpt selection |
| SB-18-ui-focused-orientation-and-context-explorer | Passed | Passed | Passed | Passed | The lab now exposes selection-quality status and broad-selection warnings without regressing the clean UI case |
| SB-19-validation-and-mcp-seam-review | Passed | Passed | Passed | Passed with residual risk | Three host cases were rerun against SharpTools. Database and UI value claims improved; common-helper fan-out still needs another tuning pass |

## Browser Validation Analytics

| Subbundle | Route | Viewport | Playwright MCP evidence | Screenshots | Result |
| --- | --- | --- | --- | --- | --- |
| SB-17-member-context-graph-and-query-api | `/context-lab?...AppDbContext...` and `/context-lab?...IClock...` | Default desktop | Final Playwright DOM extraction on 2026-04-08 captured seed, file, block, and line totals after the rebuilt app was running at `http://127.0.0.1:5501` | DOM snapshots and evaluation output only | Passed with residual helper-noise risk |
| SB-18-ui-focused-orientation-and-context-explorer | `/context-lab?...CanvasSceneHost...` | Default desktop | Final Playwright DOM extraction on 2026-04-08 captured the preserved focused UI case and the new quality label | DOM snapshots and evaluation output only | Passed |
| SB-19-validation-and-mcp-seam-review | `/context-lab` comparison matrix across the three host queries | Default desktop | Sequential Playwright rerun collected final results for `AppDbContext`, `IClock`, and `CanvasSceneHost` in one browser pass | DOM snapshots and evaluation output only | Passed |

## Analytics Review

- Final rerun results versus the previous reopen baseline:
  - Database case `AppDbContext` improved from `622 selected lines / 8 files / 15 blocks` to `139 selected lines / 5 files / 8 blocks`. The seed is now the correct `AppDbContext` type instead of a factory-adjacent false positive, and the result is materially less noisy.
  - Common-helper case `IClock` moved from a duplicate-path crash to a correct `IClock.GetUtcNow()` seed with `164 selected lines / 8 files / 17 blocks`. The failure is closed, but the helper fan-out is still too broad for a ubiquitous service.
  - UI case `CanvasSceneHost` improved from `98 selected lines / 3 files / 8 blocks` to `59 selected lines / 3 files / 6 blocks` while remaining clearly useful. This is the preserve case and it stayed strong.

## Host Validation Summary

- Baseline comparison findings before the new reopen:
  - Database case `AppDbContext` was correct but noisy.
  - Common-helper case `IClock` failed on duplicate generated-path handling.
  - UI case `CanvasSceneHost` was already useful and must be preserved.
- Final rerun after the repair cycle:
  - Database: useful first-pass context is now cheaper and cleaner, but it still includes a small amount of peripheral persistence noise.
  - Common helper: the correctness failure is fixed and the exact helper seed is now stable, but the result still carries too many consumers for a helper that is used across the host solution.
  - UI: the focused-context flow remains the better first-pass operator experience because one query yields the working file cluster immediately.

## SharpTools Comparison

- Final comparison after the repair cycle:
  - Database case:
    - Focused context now wins the first-pass context bundle. One lab query returns the main `AppDbContext` slice plus nearby collaborators in a bounded payload.
    - SharpTools is still more precise when the operator already knows the exact next symbol to inspect, but it needs multiple explicit calls to assemble the same neighborhood.
  - Common-helper case:
    - SharpTools still wins. `ViewDefinition(IClock)` plus `ListImplementations(IClock)` and targeted reference search stays cleaner than the focused-context consumer spread.
    - Focused context is no longer broken here, but it should currently hand off after the first pass instead of pretending the whole helper neighborhood is already well tuned.
  - UI case:
    - Focused context wins on operator cost because the selected file cluster is already the relevant work surface.
    - SharpTools remains the better follow-up tool once the operator wants one exact method or definition body next.

## Value Conclusion

- The feature now supports the main value claim for database and UI trouble paths: it gives an agent a compact starting bundle that is cheaper than opening whole files and cheaper than manually assembling the same neighborhood with SharpTools.
- The value claim is only partially met for ubiquitous helpers. The crash is fixed and the seed is correct, but the noise is still higher than the intended standard.
- The practical guidance is now clear:
  - Use focused context first for database and UI trouble paths.
  - Use focused context as a bounded entry point for common helpers, then hand over to SharpTools once the first correct seed is found.

## Raw Note Closure

| Raw note | Status | Proof |
| --- | --- | --- |
| Start with detailed refactoring first | Partially solved | Refactor-first baseline exists, but the focused-context comparison exposed new readability work in the application and lab slices |
| Implement the previous recommendations | Partially solved | The prior cycle shipped the requested features, but the direct comparison exposed one correctness defect and two tuning defects |
| Add focused trouble-path code context | Partially solved | The feature exists, but broad database/helper cases still need tightening |
| Start from exception or compile-error text | Solved | Unit coverage now exercises diagnostic-text and prompt-text seed resolution paths |
| Allow tags to focus the result | Solved | Prior proof still stands, but rerun evidence is pending |
| Show accordions grouped by file with selected code parts | Solved | Prior proof still stands, but rerun evidence is pending |
| Show line-count stats for file groups and the full result | Solved | Prior proof still stands, but rerun evidence is pending |
| Provide a dedicated page for tuning with workspace scope, prompt, and tags together | Solved | Prior proof still stands, but rerun evidence is pending |
| Explain what feedback the tuning page should capture | Partially solved | The next pass must also surface quality cues faster and rerun the explicit three-case rubric |
| Analyze the focused-context flow against SharpTools | Solved | The reopen captured database, helper, and UI baseline findings against SharpTools |
| Compare helpfulness and noise, not only counts | Solved | The reopen defined the comparison problem and concrete baseline evidence |
| Improve the feature based on the comparison | Solved with residual risk | Duplicate-path failures are fixed, exact-type seeding is tighter, whole-type spill is reduced, and the lab now shows quality cues; common-helper fan-out still remains the next tuning target |
| Include generic readability and structure refactoring | Solved | The focused-context pipeline is now easier to explain by responsibility across seed resolution, member expansion, excerpt assembly, and lab quality evaluation |

## Residual Risks

- Exact helper seeds are now resolved correctly, but high-reuse helpers still over-expand into too many consumers across the host solution.
- Database broad-search behavior is much better than the baseline, but persistence-adjacent factory and storage helpers can still leak into the result sooner than ideal.
- The managed watch health probe remained flaky during browser proof, so route validation relied on direct browser checks against the served page rather than the probe alone.
