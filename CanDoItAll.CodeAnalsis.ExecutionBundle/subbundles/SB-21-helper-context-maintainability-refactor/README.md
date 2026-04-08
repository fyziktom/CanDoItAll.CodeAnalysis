# SB-21 Helper context maintainability refactor

## Status

- Completed

## Objective

- Refactor the focused-context helper-mode pipeline so strategy selection, traversal, sampling, and response shaping stay readable and clearly owned.

## Covered Inputs

- Then do common refactoring to improve code maintainability
- Keep the implementation strongly typed and maintainable
- Avoid turning helper precision into another tangled heuristic cluster

## Prerequisites

- `SB-20` passed

## Exact Source References

- `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Application\Services`
- `C:\repositories\CanDoItAll.CodeAnalsis\tests\CanDoItAll.CodeAnalytics.Tests.Unit`

## Deliverables

- Clear ownership boundaries for helper classification, traversal strategy, sampling policy, and response shaping
- Reduced condition sprawl in the focused-context pipeline
- Behavior-preserving refactor proof

## Dependency Impact

- Blocks broader helper-mode work because response shaping should not be layered onto unclear strategy ownership.

## Validation Depth

- Build, full tests, and code review against ownership clarity

## Implementation Steps

1. Extract or clarify helper-mode strategy boundaries.
2. Reduce mixed responsibility between seed resolution, traversal, sampling, and payload assembly.
3. Keep behavior stable while improving maintainability.

## Do Not Do

- Do not widen scope beyond maintainability and ownership.
- Do not hide central logic in vague helper buckets.

## Acceptance Checklist

- The helper-mode pipeline is easier to explain by responsibility.
- New logic does not depend on scattered boolean condition chains.
- Build and tests still pass.

## Proof Required

- `dotnet build .\CanDoItAll.CodeAnalsis.slnx -nologo`
- `dotnet test .\CanDoItAll.CodeAnalsis.slnx -nologo`
- Refactor note in execution report

## Browser Validation Logging

- N/A in this phase

## Progression Gate

- Later helper-mode improvements may continue only after the strategy code is understandable enough to evolve safely.

## Suggested Agent Prompt

Refactor the helper-mode focused-context pipeline for ownership clarity after the minimal strategy change set is stable. Keep behavior correct while making later response shaping cheaper to reason about.
