# SB-17 Member context graph and query API

## Status

- Completed

## Objective

- Extend the canonical member-level relationship graph and focused context query surface so it can resolve free-text seeds, apply focus tags, and return bounded code excerpts grouped by file.

## Covered Inputs

- Provide agent context focused on solving trouble
- Start from the function or class around the issue
- Recursively map related functions and classes with stop limits
- Include exact source references and optional summaries
- Identify high-reuse helpers heuristically
- Start from exception text, compile-error text, or an explicit symbol name when ids are unavailable
- Bias selection with tags such as `Db`
- Return per-file excerpts and line-count stats instead of source references alone

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
- Application query implementation and tests

## Dependency Impact

- `SB-18` depends on this feature to expose real context exploration and file accordions.
- `SB-19` depends on it for the SharpTools comparison, tuning guidance, and context-savings claim.

## Validation Depth

- Build, unit tests, integration tests, and comparison-oriented sample outputs

## Implementation Steps

1. Define member relationship contracts.
2. Collect member relationships from Roslyn semantic analysis.
3. Add prompt-text seed resolution for diagnostics, symbols, and source-path hints.
4. Add tag-aware scoring and grouped excerpt output with deterministic ordering and stats.

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

## Proof Required

- Unit tests for traversal and ordering
- Integration tests against the fixture solution
- Seed-resolution and tag-behavior tests
- Example focused-context payloads that include grouped excerpts and stats

## Browser Validation Logging

- N/A in this phase

## Progression Gate

- UI work may continue only after focused context outputs are trustworthy enough to drive real navigation and visible excerpt review.

## Suggested Agent Prompt

Implement the smallest useful member graph upgrade that supports bounded trouble-path exploration from free-text seeds, explicit tags, and grouped file excerpts.
