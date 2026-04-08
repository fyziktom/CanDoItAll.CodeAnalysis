# Structured input

## Objectives

- Refactor the current standalone solution before adding more breadth.
- Make diagrams more useful by scoping and selecting relations intentionally.
- Recover more real EF schema relationships from CanDoItAll-sized solutions.
- Add focused code-context queries centered on member or type trouble paths.
- Reduce the number of SharpTools-style follow-up calls needed for orientation.
- Allow focused context to start from exception text, compile-error text, or a plain developer prompt.
- Bias traversal with explicit focus tags such as `Db`.
- Surface grouped code excerpts and stats so the user can judge whether the chosen context is precise enough.
- Provide a dedicated lab page that combines workspace selection, prompt entry, tags, and visible output for tuning.
- Compare the focused-context flow directly against SharpTools on real CanDoItAll database, helper, and UI searches.
- Improve the feature based on that comparison instead of relying on intuition.
- Include readability and structure refactoring in the focused-context implementation while preserving behavior.

## Hard constraints

- Do not modify anything in `C:\repositories\CanDoItAll`.
- Keep the repo root and solution spelling as `CanDoItAll.CodeAnalsis`.
- Keep project and namespace families as `CanDoItAll.CodeAnalytics.*`.
- Preserve the future seam to `CanDoItAll.Mcp.CodeAnalytics`.
- Keep Roslyn-first analysis as the baseline.
- Keep the application layer transport-agnostic.

## Expected proof

- Bundle validator passes for the repaired bundle.
- Code build and tests pass.
- Mermaid output renders successfully.
- Playwright proof covers the UI for the new focused exploration features.
- Host validation is rerun against `C:\repositories\CanDoItAll\CanDoItAll.slnx`.
- Final analysis explicitly compares snapshot usage against SharpTools and explains the value tradeoff.
- The focused-context lab shows accordion-based file groups, extracted code, and line-count stats from a real run.
- The validation write-up explains what feedback the tuning page should collect for future heuristic improvement.
- The comparison uses an explicit rubric for seed correctness, first-pass usefulness, noise, and interaction cost.
- The common-helper comparison no longer crashes on duplicate generated or package-backed source paths.
- The improved lab flow makes high-noise selections visible without hiding them.

## Key gaps to solve

- Oversized files are mixing source-of-truth logic, heuristics, and rendering decisions.
- Global class diagrams are too noisy to be orientation-friendly.
- EF relationship recovery is still too shallow.
- The current focused-context flow depends on explicit ids and does not resolve developer prompt text or diagnostic text.
- The current response shows source anchors but not grouped code excerpts or per-file stats.
- There is no dedicated lab page for tuning tags, prompt text, and scope choices together.
- The bundle itself is now stale against the newer focused-context request until the reopened scope is modeled explicitly.
- Broad type searches still choose constructor-heavy seeds and over-expand into consumers that are not the first useful neighborhood.
- Type-only fallback excerpts can still dump nearly whole files when a representative member would be enough.
- Shared helper searches can still fail when duplicate document paths from package-backed generated files collide during excerpt assembly.
