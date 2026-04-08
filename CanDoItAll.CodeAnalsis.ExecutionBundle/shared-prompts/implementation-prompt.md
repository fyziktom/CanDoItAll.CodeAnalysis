# Implementation prompt

Implement the current active subbundle only.

- Read the bundle root README, phase plan, and the selected subbundle README first.
- Respect the canonical source-of-truth map in `architecture/01-target-solution.md`.
- Do the smallest maintainable refactor that unlocks the subbundle objective.
- Keep the application layer transport-agnostic.
- Keep the UI SSR-first and thin.
- Keep Mermaid renderers consumers of facts, not sources of truth.
- Preserve deterministic ordering in every new query and export.
- Add or update tests with every behavioral change.
- Run the subbundle closure validation before moving to the next phase.

If a prerequisite is weak, reopen the prior subbundle instead of pushing through.
