# Assumptions and risks

## Working Assumptions

- The reopened work should repair the bundle and execute against the existing standalone repo rather than creating a second bundle root.
- The current host validation snapshot remains a valid baseline for identifying usefulness gaps, but it must be rerun after implementation.
- The focused context feature can start from Roslyn semantic analysis and existing symbol facts without needing runtime reflection.
- Function-level context will be materially useful even if the first version uses bounded static call and type dependency traversal rather than perfect whole-program flow analysis.

## Critical Path Risks

- Refactor-first work can destabilize collector behavior if canonical source-of-truth ownership is not preserved while splitting files.
- Member-context graph collection can explode in size if recursion depth, node count, or allowed relationship kinds are not bounded explicitly.
- Scoped diagrams can still be noisy if selection heuristics are not centered on connected neighborhoods and project/module boundaries.
- Reopened bundle execution can drift if old completed subbundles and new active subbundles are not separated cleanly in the phase plan.

## Validation Risks

- Mermaid syntax can still regress if new relation labels or scoped exports emit invalid identifiers.
- EF relationship recovery may appear improved on the fixture solution while still missing convention-based relations in the host repo.
- UI proof can be misleading if only the closed navigation state is tested instead of real context exploration flows.
- The context query can look useful in JSON but still fail the “saves calls and context” test unless it is compared directly with SharpTools probing.

## Reopen Triggers

- Reopen `SB-15` if any later subbundle exposes wrong source-of-truth placement or oversized files regrow without ownership cleanup.
- Reopen `SB-16` if host-solution diagrams are still noisy or EF relationships remain obviously under-reported.
- Reopen `SB-17` if the focused context query requires whole-file reading to stay understandable.
- Reopen `SB-18` if Playwright proof shows the UI cannot drive the new context exploration path clearly.
- Reopen `SB-19` if the future MCP seam thickens or if the snapshot versus SharpTools comparison still shows poor context savings.
