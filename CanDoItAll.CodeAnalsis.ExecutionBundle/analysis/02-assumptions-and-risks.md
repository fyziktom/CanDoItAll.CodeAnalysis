# Assumptions and risks

## Working Assumptions

- The reopened work should repair the bundle and execute against the existing standalone repo rather than creating a second bundle root.
- The current host validation snapshot remains a valid baseline for identifying usefulness gaps, but it must be rerun after implementation.
- The focused context feature can start from Roslyn semantic analysis and existing symbol facts without needing runtime reflection.
- Function-level context will be materially useful even if the first version uses bounded static call and type dependency traversal rather than perfect whole-program flow analysis.
- The first tag system can be heuristic and keyword-driven as long as the applied tags and their effect remain visible in the UI.
- The dedicated lab page may build or reuse snapshots synchronously for now because cache reuse keeps repeated tuning runs practical in the standalone tool.
- The next pass should reopen the existing bundle instead of creating a second bundle root because the new work is a direct consequence of the completed focused-context cycle.
- The helper-precision pass can remain incremental if it introduces a typed traversal strategy instead of rewriting the full focused-context pipeline at once.
- Relation hints should be treated as explicit narrowing hints, not as a persistent ontology or natural-language planner.
- The host `CanDoItAll.Mcp.CodeAnalytics` wrapper and CodeAnalytics MCP skill may be edited for focused-context contract exposure, but unrelated dirty host MCP changes must be left alone.

## Critical Path Risks

- Refactor-first work can destabilize collector behavior if canonical source-of-truth ownership is not preserved while splitting files.
- Member-context graph collection can explode in size if recursion depth, node count, or allowed relationship kinds are not bounded explicitly.
- Scoped diagrams can still be noisy if selection heuristics are not centered on connected neighborhoods and project/module boundaries.
- Reopened bundle execution can drift if old completed subbundles and new active subbundles are not separated cleanly in the phase plan.
- Free-text seed resolution can pick the wrong member if diagnostic text, helper names, and overloaded symbols are not scored carefully.
- Excerpt generation can become misleading if line spans are too shallow, too wide, or grouped incorrectly across files.
- Shared helper analysis can fail entirely if duplicate normalized document paths from package-backed generated files are still treated as unique dictionary keys.
- Broad type queries can remain misleading if constructor-first seed selection keeps dragging the neighborhood toward factories and consumers instead of the most explanatory member.
- High-fan-in helper symbols can remain noisy even after better seeding unless traversal and response shaping switch away from undirected neighborhood expansion.
- A helper-specific response can become harder to reason about if usage sampling, implementation lookup, and trouble-path traversal are mixed without a clear strategy boundary.
- Relation hints can create a false sense of precision if unmatched hints quietly fall back to broad helper consumers. SB-28 mitigates this for high-fan-in helper representative consumers by suppressing broad unrelated clusters when relation hints are present.
- Updating only the engine without the host MCP input and agent skill would make the feature unavailable to its primary user.

## Validation Risks

- Mermaid syntax can still regress if new relation labels or scoped exports emit invalid identifiers.
- EF relationship recovery may appear improved on the fixture solution while still missing convention-based relations in the host repo.
- UI proof can be misleading if only the closed navigation state is tested instead of real context exploration flows.
- The context query can look useful in JSON but still fail the “saves calls and context” test unless it is compared directly with SharpTools probing.
- The lab page can look convincing while still hiding noisy excerpt grouping unless screenshot review checks readability, accordion hierarchy, and stats clarity.
- Build and test validation can report false failures if lingering `testhost` processes keep binaries locked; clean reruns must isolate that tooling issue from product regressions.
- The comparison can still be misleading if only line counts are measured. The reopen must score helpfulness, noise, and operator effort explicitly.
- The helper-mode reopen can regress the strong database or UI cases if the new traversal strategy leaks into the default trouble-path path instead of being explicitly selected.
- Relation-hint metrics can look better only because context disappeared. The validation must check selected usage clusters and samples, not only lower line counts.

## Reopen Triggers

- Reopen `SB-15` if any later subbundle exposes wrong source-of-truth placement or oversized files regrow without ownership cleanup.
- Reopen `SB-16` if host-solution diagrams are still noisy or EF relationships remain obviously under-reported.
- Reopen `SB-17` if the focused context query still requires whole-file reading, cannot resolve free-text seeds reliably, or returns excerpts that are not bounded enough.
- Reopen `SB-17` if duplicate source-path collisions still break common-helper searches or if type/helper seeds still explode into broad low-value context.
- Reopen `SB-18` if Playwright proof shows the lab page cannot drive the new context exploration path clearly or if the accordion output is visually noisy.
- Reopen `SB-19` if the future MCP seam thickens, the tuning feedback remains vague, or the snapshot versus SharpTools comparison still shows poor context savings.
- Reopen `SB-20` or `SB-22` if helper seeds still explode into consumer-heavy bundles after the new strategy pass.
- Reopen `SB-23` if the rerun still cannot explain where focused context should stop and hand over to SharpTools for helper exploration.
- Reopen `SB-28` if relation hints are not visible in the response, lab, MCP input model, skill guidance, or harness metrics.
- Reopen `SB-28` if relation-hinted helper runs return broad unrelated caller clusters or silently ignore unmatched hints.
