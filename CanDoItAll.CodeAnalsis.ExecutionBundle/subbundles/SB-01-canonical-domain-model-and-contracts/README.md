# SB-01 Canonical domain model and contracts

## Status

- Completed

## Objective

- Create the portable snapshot contracts and base fact model for the standalone engine.

## Covered Inputs

- Canonical snapshot requirement
- Transport-agnostic application rule

## Prerequisites

- `SB-00` completed
- `SB-00A` completed

## Exact Source References

- `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Domain`
- `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Abstractions`
- `C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.ExecutionBundle\overview\04-canonical-snapshot-model.md`

## Deliverables

- Snapshot contracts
- Domain fact records
- Query and response abstractions

## Dependency Impact

- Every later collector, renderer, and UI response depends on these contracts.

## Validation Depth

- Build plus serialization and architecture tests

## Implementation Steps

1. Model portable snapshot contracts.
2. Add core fact types.
3. Add abstraction contracts used by application and UI.

## Do Not Do

- Do not place transport or UI logic in the domain layer.

## Acceptance Checklist

- Domain and abstraction projects compile cleanly.
- Snapshot contracts are serializable and test-covered.

## Proof Required

- Unit tests for serialization
- Architecture tests for layering

## Browser Validation Logging

- N/A

## Progression Gate

- Canonical contracts are stable enough for collectors to target them.

## Suggested Agent Prompt

Implement only the canonical contracts and preserve strict layering.
