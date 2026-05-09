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
| Start from exception or compile-error text, not only explicit ids | REQ-016, REQ-017, REQ-019 | SB-17, SB-18 | Seed-resolution tests and lab-page proof | Diagnostic and prompt text must map to a bounded seed |
| Allow tags such as `Db` to bias the result | REQ-017, REQ-020 | SB-17, SB-18, SB-19 | Unit tests, lab screenshots, tuning notes | First version may be heuristic as long as behavior is explicit |
| Show selected code parts below as accordions per file | REQ-018, REQ-019 | SB-17, SB-18 | Query payload tests and Playwright proof | Source links alone are no longer enough |
| Show how many lines each file group contributes and overall stats | REQ-018, REQ-019 | SB-17, SB-18 | UI assertions and screenshot review | Needed to judge context cost directly |
| Provide a page where solution plus optional project, prompt, and tags can be tested together | REQ-019, REQ-020 | SB-18, SB-19 | Web tests, Playwright flow, tuning write-up | Dedicated tuning surface is part of the requested value |
| Explain what feedback is needed to tune the heuristics | REQ-020 | SB-19 | Closure write-up | Validation must teach the next iteration |
| Analyze the focused-context flow against SharpTools on CanDoItAll | REQ-021, REQ-026 | SB-19 | Three-case comparison matrix, SharpTools call log, rerun write-up | Must compare database, helper, and UI cases explicitly |
| Simulate one database, one common-helper, and one UI search | REQ-021, REQ-023, REQ-024 | SB-17, SB-18, SB-19 | Host lab runs, SharpTools probes, browser proof | The quality rubric must make these searches judgeable |
| Compare not only content amount but also helpfulness and noise | REQ-021, REQ-023, REQ-026 | SB-19 | Explicit rubric and evidence-based analysis | Call count alone is not enough |
| Improve the feature based on the comparison | REQ-022, REQ-023, REQ-024 | SB-17, SB-18, SB-19 | Tests, browser proof, rerun comparison | Must close the helper failure and tighten noisy cases |
| Include generic readability and structure refactoring | REQ-025 | SB-15, SB-17, SB-18 | Code review, build, tests | Refactor must stay small and behavior-preserving |
| Make helpers like `IClock` more surgical and precise | REQ-027, REQ-028, REQ-029, REQ-031, REQ-032 | SB-20, SB-22, SB-23 | Host helper rerun, SharpTools comparison, focused-context payload review | High-fan-in helper mode is the new reopen target |
| Start with the minimal change set first | REQ-027, REQ-028, REQ-030 | SB-20 | Unit tests, focused host rerun, entry and closure gate notes | Sequencing matters; do not jump straight to a wider redesign |
| Then refactor for maintainability | REQ-030 | SB-21 | Build, tests, code review, ownership review | Helper-mode logic must not become another tangled heuristic slice |
| Then add the broader helper-mode improvements | REQ-029, REQ-031, REQ-032 | SB-22, SB-23 | UI proof, host rerun, final SharpTools comparison | Broader improvements come only after the minimal foundation is stable |
| Implement the comparison-bundle improvements and revalidate | REQ-033, REQ-034, REQ-035, REQ-036, REQ-037, REQ-038 | SB-24, SB-25, SB-26, SB-27 | Unit tests, lab proof, rerun harness output, host comparison matrix | This reopen is driven by the closed SharpTools comparison bundle |
| Large projects overload agent context too easily | REQ-039, REQ-040, REQ-043, REQ-044 | SB-28 | Relation-hinted tests, harness metrics, browser proof | Relation hints must reduce context breadth without hiding caller breadth |
| Complete scans should be specific cases | REQ-042, REQ-044, REQ-045 | SB-28 | Skill guidance, scoped snapshot UI/MCP review | Snapshot building can remain broad, but agents need a narrower default investigation sequence |
| Ask for usages with tags like `db` or `EntityFramework` | REQ-039, REQ-040, REQ-041 | SB-28 | Unit tests and lab proof | Tag aliases and relation hints must be explicit request inputs |
| Ask for a helper plus related classes or components | REQ-039, REQ-040, REQ-043 | SB-28 | High-fan-in helper relation-hint test and harness scenario | This is the main new walking behavior |
| Combine tool and agent skill as a bundle | REQ-041, REQ-042, REQ-045 | SB-28 | Host MCP build and skill update | Skill guidance must match the shipped contract |
| Add basic UI and measure results | REQ-041, REQ-043 | SB-28 | Web test, Playwright proof, comparison harness output | The lab is the no-restart control surface |

## Closure note

- Final proof for `REQ-033` through `REQ-038` is captured in:
  - [03-post-implementation-comparison.md](C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.ExecutionBundle\analysis\03-post-implementation-comparison.md)
  - [01-execution-report.md](C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.ExecutionBundle\reviews\01-execution-report.md)
  - [focused-context-summary.json](C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.ExecutionBundle\analysis\post-implementation-focused-context\focused-context-summary.json)
- `REQ-039` through `REQ-045` are owned by SB-28 and are complete with relation-hint implementation proof recorded.
- Final proof for `REQ-039` through `REQ-045` is captured in:
  - [04-relation-hinted-focused-context.md](C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.ExecutionBundle\analysis\04-relation-hinted-focused-context.md)
  - [01-execution-report.md](C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.ExecutionBundle\reviews\01-execution-report.md)
  - [focused-context-summary.json](C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.ExecutionBundle\analysis\relation-hinted-focused-context\focused-context-summary.json)
