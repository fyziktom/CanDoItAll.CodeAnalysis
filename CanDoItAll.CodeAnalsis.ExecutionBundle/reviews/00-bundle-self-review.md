# Bundle self review

## QA review

- Raw request and follow-up prompt are preserved in `inputs/`.
- Normalized requirements explicitly cover refactor-first work, scoped diagrams, EF recovery, focused context queries, free-text entry, tags, grouped excerpts, UI exposure, and validation.
- Every new requirement maps to an owning subbundle and proof method in traceability.
- Browser-proof expectations are explicit for the UI and host-validation slices.
- The reopened comparison request is preserved as raw input instead of being collapsed into the earlier closure notes.

## Senior C# Blazor architect review

- The phase sequence avoids a big-bang rewrite by isolating refactor, data recovery, query graph, UI, and closure.
- Source-of-truth ownership is explicit and keeps Roslyn collection, analysis, rendering, and UI separated.
- The plan acknowledges that member-context navigation is a new canonical fact layer, not just a UI filter.
- The reopened scope keeps prompt resolution, tag heuristics, and excerpt grouping in the application layer instead of hiding them inside Razor pages.
- The persistence and diagram subbundles are separated from the member-context work so regressions are easier to localize.
- The new reopen keeps the comparison-driven improvements small: fix the hard helper failure, tighten broad seed behavior, preserve the good UI case, and refactor the focused-context code for readability.

## Senior manager review

- The critical path is obvious: refactor foundation first, then data quality and query primitives, then UI, then proof.
- Dependency ordering is explicit in `plan/01-phase-plan.md`.
- The workbook remains the planning surface for user stories, issues, and validation ownership.
- The closure phase explicitly includes the value comparison against SharpTools, the future MCP seam review, and what user feedback must be captured for the next tuning pass.
- The new reopen still has a short critical path because it reuses the existing bundle instead of inventing a second initiative.
