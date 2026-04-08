# SB-03 Symbol indexing and XML documentation

## Status

- Completed

## Objective

- Index namespaces, types, members, and XML summaries into canonical symbol facts.

## Covered Inputs

- Symbol and docs fact collection
- Optional summary support for later context output

## Prerequisites

- `SB-02` completed

## Exact Source References

- `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Facts\Symbols`
- `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Facts\Documentation`
- `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Domain\Facts`

## Deliverables

- Namespace facts
- Type facts
- Member facts
- XML normalization

## Dependency Impact

- Later dependency, persistence, and focused-context work all depend on symbol fidelity.

## Validation Depth

- Build and unit coverage for symbol collection and documentation normalization

## Implementation Steps

1. Walk Roslyn symbols from the loaded workspace.
2. Normalize documentation summaries.
3. Persist stable ids and deterministic ordering.

## Do Not Do

- Do not mix semantic collection with rendering logic.

## Acceptance Checklist

- Symbol facts include source references.
- XML summaries are normalized and optional.

## Proof Required

- Unit tests for symbol collection and documentation normalization

## Browser Validation Logging

- N/A

## Progression Gate

- Downstream collectors can trust the symbol catalog.

## Suggested Agent Prompt

Collect only canonical symbol facts and summaries. Do not add navigation UI in this phase.
