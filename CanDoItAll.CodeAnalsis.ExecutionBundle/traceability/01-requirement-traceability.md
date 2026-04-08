# Requirement traceability

| Raw note | Normalized requirements | Owning subbundle | Planned proof | Notes |
| --- | --- | --- | --- | --- |
| Start with detailed refactoring first | REQ-001, REQ-002, REQ-003, REQ-015 | SB-15 | Build, tests, hotspot review, refactor report | Refactor precedes feature widening |
| Analyze whole solution and find architecture gaps | REQ-001, REQ-003, REQ-015 | SB-15 | Current-state analysis, workbook, bundle plan | Must use real repo state |
| Focus on very long files, sources of truth, isolation of helpers | REQ-002, REQ-003 | SB-15 | File-length audit, ownership split, code review | Canonical refactor rule |
| Implement the previous recommendations | REQ-004, REQ-005, REQ-006, REQ-007, REQ-008, REQ-009 | SB-16, SB-17, SB-18, SB-19 | Host rerun, Playwright proof, comparison write-up | Carried from the prior architecture assessment |
| Class diagrams should contain useful relations, not noisy globals | REQ-004, REQ-014 | SB-16 | Mermaid render proof and host usefulness review | Scoped outputs are required |
| Improve database schema search and relation recovery | REQ-005, REQ-014 | SB-16 | Host persistence counts and ER review | Must test against CanDoItAll |
| Provide agent context focused on solving trouble | REQ-006, REQ-007, REQ-008, REQ-009 | SB-17, SB-18, SB-19 | Focused query tests, UI proof, SharpTools comparison | Member graph is the missing primitive |
| Do not force whole-file reading when a few members are enough | REQ-006, REQ-007, REQ-009 | SB-17, SB-19 | Bounded context result and call-count comparison | Direct comparison against SharpTools style probing |
| Recursive tree from function or bug location | REQ-006, REQ-007 | SB-17 | Depth-limited traversal tests | Requires member relationship model |
| Output exact file references and optional summaries | REQ-007, REQ-008 | SB-17, SB-18 | Query payload tests and UI rendering proof | Supports selective deeper reads |
| Identify high-reuse helpers for temporary memory reuse | REQ-009 | SB-17, SB-19 | Focused context response and comparison analysis | First version can be heuristic, not a persistent skill engine |
