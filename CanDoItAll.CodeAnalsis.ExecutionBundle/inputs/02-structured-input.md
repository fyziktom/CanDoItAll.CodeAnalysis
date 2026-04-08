# Structured input

## Objectives

- Refactor the current standalone solution before adding more breadth.
- Make diagrams more useful by scoping and selecting relations intentionally.
- Recover more real EF schema relationships from CanDoItAll-sized solutions.
- Add focused code-context queries centered on member or type trouble paths.
- Reduce the number of SharpTools-style follow-up calls needed for orientation.

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

## Key gaps to solve

- Oversized files are mixing source-of-truth logic, heuristics, and rendering decisions.
- Global class diagrams are too noisy to be orientation-friendly.
- EF relationship recovery is still too shallow.
- There is no member-to-member trouble-path graph yet.
- The bundle itself was not executable under the current workflow contract.
