# Normalized requirements

## Functional requirements

- `REQ-001`: Perform a refactor-first pass across the standalone solution before widening functionality.
- `REQ-002`: Identify and reduce oversized file hotspots by splitting helpers and heuristics along canonical ownership boundaries.
- `REQ-003`: Preserve clear sources of truth for domain facts, renderers, application orchestration, and UI composition.
- `REQ-004`: Replace whole-solution-only class diagram usefulness with scoped and context-aware diagram strategies.
- `REQ-005`: Improve EF persistence analysis so it finds more real database relationships on `C:\repositories\CanDoItAll\CanDoItAll.slnx`.
- `REQ-006`: Add a focused code-context feature that can start from a type or member and expand through related members and types with bounded depth.
- `REQ-007`: Include exact source references in focused context output so an agent can jump to code only when needed.
- `REQ-008`: Expose focused context through the standalone application layer and SSR-first UI.
- `REQ-009`: Keep outputs useful for future MCP integration and token-saving orientation flows.
- `REQ-016`: Resolve focused-context seeds from free-text inputs such as exception text, compile-error text, or a symbol name when explicit ids are not available.
- `REQ-017`: Support optional focus tags that bias the traversal toward relevant parts of the codebase without making the graph unbounded.
- `REQ-018`: Return grouped per-file excerpts with selected code blocks, source anchors, and line-count stats so the result is reviewable without whole-file loading.
- `REQ-019`: Provide a dedicated tuning UI where the user can select a solution or project scope, enter prompt text, add tags, run analysis, and inspect accordion-based file results.
- `REQ-020`: Record enough visible evidence in the lab flow that future tuning feedback can say which excerpts were helpful or noisy.
- `REQ-021`: Evaluate the focused-context flow against SharpTools on at least one database, one common-helper, and one UI search against `C:\repositories\CanDoItAll\CanDoItAll.slnx` using an explicit quality rubric.
- `REQ-022`: Eliminate focused-context failures caused by duplicate normalized source paths such as package-backed generated files reused across projects.
- `REQ-023`: Reduce noise for broad type and helper searches by improving seed-member choice, fan-out control, and representative excerpt selection.
- `REQ-024`: Preserve or improve first-pass usefulness for narrow UI searches while tightening noisy database and helper cases.
- `REQ-025`: Refactor the focused-context application and UI code to improve readability, structure, and standard best practice without widening the transport seam.
- `REQ-027`: Add an explicit focused-context intent or precision mode so high-fan-in helper exploration does not use the same traversal shape as trouble-path debugging.
- `REQ-028`: Detect helper-like seeds such as small interfaces or utilities with broad consumer spread and reduce noise through directional traversal and stricter stop rules.
- `REQ-029`: Return surgical helper outputs that emphasize contract definition, implementation types, and sampled or summarized usages instead of bundling every consumer into the main excerpt set.
- `REQ-030`: Refactor the focused-context pipeline so seed classification, traversal strategy, sampling, and response shaping remain maintainable and clearly owned.
- `REQ-031`: Validate the helper-precision pass against the host helper cases and compare the resulting operator effort and noise against SharpTools again.

## Quality requirements

- `REQ-010`: Preserve Roslyn-first analysis as the baseline.
- `REQ-011`: Keep facts, insights, diagnostics, and renderers separate.
- `REQ-012`: Keep the application layer transport-agnostic.
- `REQ-013`: Keep the future `CanDoItAll.Mcp.CodeAnalytics` seam thin.
- `REQ-014`: Validate with builds, tests, Mermaid rendering, Playwright proof, and host-solution comparison.
- `REQ-026`: Validate the improvement pass with the same three-case comparison matrix so the benefit is measured instead of assumed.
- `REQ-032`: Preserve the earlier improvements for database and UI trouble-path flows while adding the more surgical helper mode.
- `REQ-015`: Update the bundle, workbook, traceability, and execution evidence to the current workflow standard.

## Non-goals for this cycle

- `NONGOAL-001`: Do not build a full replacement for SharpTools exact-source navigation.
- `NONGOAL-002`: Do not modify the host CanDoItAll repo.
- `NONGOAL-003`: Do not implement runtime reflection-based or debugger-based live analysis.
- `NONGOAL-004`: Do not build a general-purpose prompt interpreter or a persistent tag taxonomy engine in this cycle.
- `NONGOAL-005`: Do not replace SharpTools exact-definition workflows; helper mode should complement SharpTools, not clone it.
