# SB-17 Member context graph and query API

## Status

- Completed

## Objective

- Repair the focused-context engine so it survives shared-helper searches, chooses better broad-search seeds, and returns more representative excerpts instead of near-whole-file spill.

## Covered Inputs

- Provide agent context focused on solving trouble
- Start from the function or class around the issue
- Recursively map related functions and classes with stop limits
- Include exact source references and optional summaries
- Identify high-reuse helpers heuristically
- Start from exception text, compile-error text, or an explicit symbol name when ids are unavailable
- Bias selection with tags such as `Db`
- Return per-file excerpts and line-count stats instead of source references alone
- Analyze a database, common-helper, and UI case against SharpTools
- Eliminate duplicate-path crashes
- Tighten broad type and helper searches without regressing narrow UI usefulness

## Prerequisites

- `SB-15` completed
- `SB-03` and `SB-10` remain trusted

## Exact Source References

- `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Domain`
- `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Facts`
- `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Application`
- `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Abstractions`

## Deliverables

- Canonical member relationship facts
- Bounded focused-context query contracts
- Seed-resolution heuristics for prompt and diagnostic text
- Tag-aware selection and grouped excerpt output with stats
- Duplicate-safe excerpt assembly
- Better seed-member and representative-excerpt heuristics
- Application query implementation and tests

## Dependency Impact

- `SB-18` depends on this feature to expose real context exploration and file accordions.
- `SB-19` depends on it for the SharpTools comparison, tuning guidance, and context-savings claim.

## Validation Depth

- Build, unit tests, integration tests, and comparison-oriented sample outputs

## Implementation Steps

1. Fix duplicate normalized-path handling in focused-context assembly.
2. Improve type and member seed selection so broad type-name queries do not default to constructor-heavy neighborhoods.
3. Replace noisy type-only excerpt fallbacks with more representative members or tighter header slices.
4. Tighten fan-out rules enough to help database and helper cases while preserving the strong UI case.

## Do Not Do

- Do not build an unbounded whole-program graph.
- Do not require whole-file loading just to understand the result.

## Acceptance Checklist

- Focused context can start from at least one type or member identifier.
- Results are bounded by depth and node count.
- Every node includes source references.
- Prompt text can resolve to a bounded seed without explicit ids in at least one diagnostic-oriented case.
- File-grouped excerpts expose enough code to judge the result without reading the whole file.
- Stats show file count, excerpt count, and selected line totals.
- The helper comparison case no longer crashes.
- The database comparison case is measurably tighter than the baseline reopen.
- The UI comparison case remains useful after the tightening pass.

## Proof Required

- Unit tests for traversal and ordering
- Integration tests against the fixture solution
- Seed-resolution and tag-behavior tests
- Regression tests for duplicate-path safety and broad-seed behavior
- Example focused-context payloads that include grouped excerpts and stats

## Browser Validation Logging

- N/A in this phase

## Progression Gate

- UI work may continue only after focused context outputs are trustworthy enough to drive real navigation and visible excerpt review.

## Completion Notes

- Duplicate normalized document paths are now merged safely during excerpt assembly.
- Exact-type gating prevents unrelated constructor consumers and factory wrappers from outranking direct type-name intent.
- Whole-type fallback spill was replaced with representative member excerpts or compact type-header slices.
- Regression coverage now includes duplicate-path safety and behavioral-member preference for type-name queries.

## Suggested Agent Prompt

Implement the smallest useful focused-context engine repair that closes the helper crash, tightens broad seeds, and keeps the strong UI case intact.
