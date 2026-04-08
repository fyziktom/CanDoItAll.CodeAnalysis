# Execution report

## Status

- The bundle is reopened again on 2026-04-07 for comparison-driven repairs after the direct SharpTools analysis exposed one correctness defect and two tuning defects.
- The comparison-driven repair cycle completed on 2026-04-08 with updated build, test, browser, and SharpTools evidence.
- Prior evidence from the earlier reopened pass is retained only as the baseline for before-versus-after comparison.
- The bundle is reopened again on 2026-04-08 for helper-precision work after the residual `IClock` analysis showed that high-fan-in helpers still need a dedicated surgical mode.
- The helper-precision reopen completed on 2026-04-08 with typed intent and precision controls, targeted helper strategy ownership, clustered usage summaries, refreshed browser proof, and full build and test validation.

## Subbundle Gate Results

| Subbundle | Entry gate | Closure gate | Downstream dependencies checked | Progression result | Notes |
| --- | --- | --- | --- | --- | --- |
| SB-15-refactor-foundation-and-canonical-ownership | Passed | Passed | Passed | Passed | Focused-context ownership is now split more clearly across seed resolution, member expansion, excerpt assembly, and lab quality helpers |
| SB-16-scoped-diagrams-and-persistence-recovery | Passed | Passed | Passed | Passed | Trusted baseline from the earlier reopened pass |
| SB-17-member-context-graph-and-query-api | Passed | Passed | Passed | Passed | Duplicate normalized-path crashes are covered, exact-type gating is tighter, and whole-type spill was replaced by representative excerpt selection |
| SB-18-ui-focused-orientation-and-context-explorer | Passed | Passed | Passed | Passed | The lab now exposes selection-quality status and broad-selection warnings without regressing the clean UI case |
| SB-19-validation-and-mcp-seam-review | Passed | Passed | Passed | Passed with residual risk | Three host cases were rerun against SharpTools. Database and UI value claims improved; common-helper fan-out still needs another tuning pass |
| SB-20-helper-surgical-minimal-change-set | Passed | Passed | Passed | Passed | `FocusedContextIntent` and `FocusedContextPrecision` are strongly typed, high-fan-in helper seeds are detected, and helper mode no longer rides the default undirected traversal path |
| SB-21-helper-context-maintainability-refactor | Passed | Passed | Passed | Passed | Strategy ownership is now explicit in `CodeAnalyticsApplicationService.Context.Strategy.cs`, and member ordering, implementation recovery, sampling, and usage clustering are separated cleanly from seed resolution |
| SB-22-helper-precision-response-shaping-and-ui | Passed | Passed | Passed | Passed | Helper responses now carry strategy explanation, implementation types, usage summaries, and lab controls for intent and precision with browser proof on both auto and explicit usage-summary flows |
| SB-23-helper-precision-validation-and-sharptools-rerun | Passed | Passed | Passed | Passed with operational caveat | Host reruns prove the narrower helper result and preserve DB/UI behavior; browser truth passed even though the managed watch health probe remained unreliable |

## Browser Validation Analytics

| Subbundle | Route | Viewport | Playwright MCP evidence | Screenshots | Result |
| --- | --- | --- | --- | --- | --- |
| SB-17-member-context-graph-and-query-api | `/context-lab?...AppDbContext...` and `/context-lab?...IClock...` | Default desktop | Final Playwright DOM extraction on 2026-04-08 captured seed, file, block, and line totals after the rebuilt app was running at `http://127.0.0.1:5501` | DOM snapshots and evaluation output only | Passed with residual helper-noise risk |
| SB-18-ui-focused-orientation-and-context-explorer | `/context-lab?...CanvasSceneHost...` | Default desktop | Final Playwright DOM extraction on 2026-04-08 captured the preserved focused UI case and the new quality label | DOM snapshots and evaluation output only | Passed |
| SB-19-validation-and-mcp-seam-review | `/context-lab` comparison matrix across the three host queries | Default desktop | Sequential Playwright rerun collected final results for `AppDbContext`, `IClock`, and `CanvasSceneHost` in one browser pass | DOM snapshots and evaluation output only | Passed |
| SB-22-helper-precision-response-shaping-and-ui | `/context-lab?...IClock&intent=Auto...` and `/context-lab?...IClock&intent=UsageSummary...` | `1600x1000` | Playwright DOM extraction confirmed `Definition` + `Surgical` in auto mode, `Usage summary` + `Surgical` in explicit mode, implementation rendering, and clustered caller summaries | `focused-context-lab-iclock-auto.png`, `focused-context-lab-iclock-usage-summary.png` | Passed |
| SB-23-helper-precision-validation-and-sharptools-rerun | `/context-lab?...AppDbContext...`, `/context-lab?...CanvasSceneHost...`, plus the two `IClock` runs above | `1600x1000` | Playwright DOM extraction captured preserve-case `AppDbContext` and `CanvasSceneHost` metrics plus the narrower helper modes on the same rerun day | Helper screenshots plus DOM evaluation output for DB and UI preserve cases | Passed with watch-health caveat |

## Analytics Review

- Final rerun results versus the previous reopen baseline:
  - Database case `AppDbContext` stayed healthy at `77 selected lines` with `Mixed` quality, `Trouble path` intent, and `Balanced` precision on the 2026-04-08 preserve rerun.
  - Common-helper case `IClock` now auto-resolves to `Definition` + `Surgical` with `48 selected lines / 4 files / 6 blocks`, `111 callers`, `4 shown clusters`, and `33 omitted callers`. This is materially tighter than the prior `164 selected lines / 8 files / 17 blocks` baseline.
  - Explicit helper `Usage summary` mode reduces the same `IClock` seed to `8 selected lines / 1 file / 2 blocks` while preserving the `111 callers` and `4 / 16` cluster summary in the UI.
  - UI case `CanvasSceneHost` stayed healthy at `50 selected lines` with `Focused` quality, `Trouble path` intent, and `Balanced` precision on the preserve rerun.
- The helper reopen is therefore closed as a real precision improvement rather than another scoring-only adjustment.

## Host Validation Summary

- Baseline comparison findings before the helper reopen:
  - Database case `AppDbContext` was already improved and needed preservation only.
  - Common-helper case `IClock` was correct but still too broad.
  - UI case `CanvasSceneHost` was already useful and had to remain untouched.
- Final rerun after the helper-precision pass:
  - Database: preserved. The result remains a compact first-pass bundle and did not drift into a broader helper-style payload.
  - Common helper: improved. Auto mode now shows contract, implementation, and representative consumer files instead of indiscriminate consumer spread, while explicit `Usage summary` mode suppresses consumer excerpts entirely.
  - UI: preserved. The result stayed focused and still behaves like a direct first-pass work surface.

## SharpTools Comparison

- Final comparison after the helper-precision pass:
  - Database case:
    - Focused context still wins the first-pass context bundle. One lab query returns a bounded `AppDbContext` slice without requiring multiple tool calls.
    - SharpTools remains the better follow-up once the operator wants one exact symbol body next.
  - Common-helper case:
    - SharpTools still wins absolute precision for single-symbol drill-down because `ViewDefinition(IClock)` plus `ListImplementations(IClock)` stays narrower than any bundled consumer view.
    - Focused context now closes much more of the gap. Auto helper mode gives a useful first-pass bundle, and explicit `Usage summary` mode cleanly separates breadth from excerpts instead of blending the two.
  - UI case:
    - Focused context still wins on operator cost because the selected file cluster is already the relevant work surface.
    - SharpTools remains the better second step after the first cluster is found.

## Value Conclusion

- The feature now supports the value claim for database, UI, and common-helper first-pass navigation:
  - database and UI trouble paths remain compact and directly useful,
  - high-fan-in helpers now have a real low-noise path instead of one blended consumer-heavy output.
- The practical guidance is now clearer and stronger:
  - Use focused context first for database and UI trouble paths.
  - Use focused context auto mode for helper orientation when you want contract, implementation, and representative consumers together.
  - Use focused context `Usage summary` mode when you want helper breadth without consumer excerpts.
  - Hand off to SharpTools once you know the exact symbol body or implementation body you want next.

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
| Make helpers like `IClock` more surgical and precise | Solved | Auto `IClock` now lands at `48 selected lines / 4 files / 6 blocks` with clustered usage summary instead of the old broad spread |
| Start with the minimal change set first | Solved | Typed intent and precision contracts plus helper-seed detection and targeted strategy routing landed before broader payload and UI work |
| Then refactor for maintainability | Solved | Strategy ownership is now separated into a dedicated focused-context strategy partial with clearer responsibility boundaries |
| Then add the broader helper-mode improvements | Solved | Implementation types, usage summaries, representative-consumer shaping, lab controls, and browser proof all shipped in the same reopen |

## Residual Risks

- Helper auto mode is now materially narrower, but it still samples only the top `4 / 16` clusters for `IClock`, so deeper helper breadth still depends on follow-up queries rather than one all-in result.
- Database broad-search behavior remains healthy, but it still carries a `Mixed` quality label rather than a fully `Focused` one on the preserve rerun.
- The managed watch health probe remained flaky during browser proof, so route validation relied on direct browser checks against the served page rather than the probe alone.
