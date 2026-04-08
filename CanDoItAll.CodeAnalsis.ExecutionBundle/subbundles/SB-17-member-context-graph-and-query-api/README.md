# SB-17 Member context graph and query API

## Status

- Completed

## Objective

- Add the canonical member-level relationship graph and focused context query surface that can expand from a type or member through bounded related code.

## Covered Inputs

- Provide agent context focused on solving trouble
- Start from the function or class around the issue
- Recursively map related functions and classes with stop limits
- Include exact source references and optional summaries
- Identify high-reuse helpers heuristically

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
- Application query implementation and tests

## Dependency Impact

- `SB-18` depends on this feature to expose real context exploration.
- `SB-19` depends on it for the SharpTools comparison and context-savings claim.

## Validation Depth

- Build, unit tests, integration tests, and comparison-oriented sample outputs

## Implementation Steps

1. Define member relationship contracts.
2. Collect member relationships from Roslyn semantic analysis.
3. Build bounded traversal and query responses with deterministic ordering.

## Do Not Do

- Do not build an unbounded whole-program graph.
- Do not require whole-file loading just to understand the result.

## Acceptance Checklist

- Focused context can start from at least one type or member identifier.
- Results are bounded by depth and node count.
- Every node includes source references.

## Proof Required

- Unit tests for traversal and ordering
- Integration tests against the fixture solution
- Example focused-context payloads

## Browser Validation Logging

- N/A in this phase

## Progression Gate

- UI work may continue only after focused context outputs are trustworthy enough to drive real navigation.

## Suggested Agent Prompt

Implement the smallest useful member graph that supports bounded trouble-path exploration with source links.
