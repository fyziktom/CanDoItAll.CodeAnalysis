# SB06 Semantic Invariants

- The desktop sandbox remains a large-screen tool; no small or medium responsive tuning was introduced as a goal.
- UI decomposition is rendering-only: application service contracts, query parameters, snapshot IDs, and focused-context response semantics are preserved.
- `ContextLab` and snapshot focused-context views share components for selected files, usage summary, and details rather than duplicating dense rendering logic.
- Browser proof covers home, operation details, dashboard, context lab, focused context, symbols search/detail, exports, and persistence at `1600x1000`.
- File-length validation now passes; remaining large files are warnings below the hard threshold, not release-blocking failures.
