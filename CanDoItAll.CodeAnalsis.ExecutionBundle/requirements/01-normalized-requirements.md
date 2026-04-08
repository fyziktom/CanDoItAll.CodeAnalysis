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

## Quality requirements

- `REQ-010`: Preserve Roslyn-first analysis as the baseline.
- `REQ-011`: Keep facts, insights, diagnostics, and renderers separate.
- `REQ-012`: Keep the application layer transport-agnostic.
- `REQ-013`: Keep the future `CanDoItAll.Mcp.CodeAnalytics` seam thin.
- `REQ-014`: Validate with builds, tests, Mermaid rendering, Playwright proof, and host-solution comparison.
- `REQ-015`: Update the bundle, workbook, traceability, and execution evidence to the current workflow standard.

## Non-goals for this cycle

- `NONGOAL-001`: Do not build a full replacement for SharpTools exact-source navigation.
- `NONGOAL-002`: Do not modify the host CanDoItAll repo.
- `NONGOAL-003`: Do not implement runtime reflection-based or debugger-based live analysis.
- `NONGOAL-004`: Do not build a general-purpose prompt interpreter or a persistent tag taxonomy engine in this cycle.
