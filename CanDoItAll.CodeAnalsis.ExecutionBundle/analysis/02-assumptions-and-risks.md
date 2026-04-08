# Assumptions and risks

## Working Assumptions

- The reopened work should repair the bundle and execute against the existing standalone repo rather than creating a second bundle root.
- The current host validation snapshot remains a valid baseline for identifying usefulness gaps, but it must be rerun after implementation.
- The focused context feature can start from Roslyn semantic analysis and existing symbol facts without needing runtime reflection.
- Function-level context will be materially useful even if the first version uses bounded static call and type dependency traversal rather than perfect whole-program flow analysis.
- The first tag system can be heuristic and keyword-driven as long as the applied tags and their effect remain visible in the UI.
- The dedicated lab page may build or reuse snapshots synchronously for now because cache reuse keeps repeated tuning runs practical in the standalone tool.

## Critical Path Risks

- Refactor-first work can destabilize collector behavior if canonical source-of-truth ownership is not preserved while splitting files.
- Member-context graph collection can explode in size if recursion depth, node count, or allowed relationship kinds are not bounded explicitly.
- Scoped diagrams can still be noisy if selection heuristics are not centered on connected neighborhoods and project/module boundaries.
- Reopened bundle execution can drift if old completed subbundles and new active subbundles are not separated cleanly in the phase plan.
- Free-text seed resolution can pick the wrong member if diagnostic text, helper names, and overloaded symbols are not scored carefully.
- Excerpt generation can become misleading if line spans are too shallow, too wide, or grouped incorrectly across files.

## Validation Risks

- Mermaid syntax can still regress if new relation labels or scoped exports emit invalid identifiers.
- EF relationship recovery may appear improved on the fixture solution while still missing convention-based relations in the host repo.
- UI proof can be misleading if only the closed navigation state is tested instead of real context exploration flows.
- The context query can look useful in JSON but still fail the “saves calls and context” test unless it is compared directly with SharpTools probing.
- The lab page can look convincing while still hiding noisy excerpt grouping unless screenshot review checks readability, accordion hierarchy, and stats clarity.
- Build and test validation can report false failures if lingering `testhost` processes keep binaries locked; clean reruns must isolate that tooling issue from product regressions.

## Reopen Triggers

- Reopen `SB-15` if any later subbundle exposes wrong source-of-truth placement or oversized files regrow without ownership cleanup.
- Reopen `SB-16` if host-solution diagrams are still noisy or EF relationships remain obviously under-reported.
- Reopen `SB-17` if the focused context query still requires whole-file reading, cannot resolve free-text seeds reliably, or returns excerpts that are not bounded enough.
- Reopen `SB-18` if Playwright proof shows the lab page cannot drive the new context exploration path clearly or if the accordion output is visually noisy.
- Reopen `SB-19` if the future MCP seam thickens, the tuning feedback remains vague, or the snapshot versus SharpTools comparison still shows poor context savings.
