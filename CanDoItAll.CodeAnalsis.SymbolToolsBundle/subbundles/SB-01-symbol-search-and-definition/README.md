# SB-01 Symbol search and definition

## Status

- Completed

## Objective

- Add explicit symbol search and exact definition viewing for both type and member targets.

## Covered Inputs

- tools that we are missing and sharptools has them
- both ways how agent can reach informations

## Exact Source References

- C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Abstractions
- C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Application\Services
- C:\repositories\CanDoItAll.CodeAnalsis\tests\CanDoItAll.CodeAnalytics.Tests.Unit\ApplicationFacts.cs

## Prerequisites

- SB-00 is complete and trusted.

## Deliverables

- Symbol search contracts and implementation
- Symbol definition contracts and implementation
- Unit coverage for type and member definition cases

## Dependency Impact

- Enables the UI start-point workflow and provides the base target resolution required by later symbol drilldowns.

## Validation Depth

- Unit tests
- Manual artifact inspection on the fixture snapshot

## Implementation Steps

1. Add explicit search and definition contracts.
2. Implement deterministic symbol search over the snapshot facts.
3. Implement type and member definition viewing with source excerpts.
4. Add focused unit coverage for type and member cases.

## Do Not Do

- Do not hide invalid search modes behind silent fallbacks.
- Do not make the definition response depend on focused-context selection.

## Acceptance Checklist

- Type targets can be searched and resolved.
- Member targets can be searched and resolved.
- Definition output includes source path and line metadata.
- Unit tests cover both type and member definitions.

## Proof Required

- Passing unit tests
- Stored output samples or comparison notes

## Browser Validation Logging

- N/A in this phase

## Progression Gate

- Downstream work may continue only when search resolves stable symbol targets and definition excerpts are correct.

## Suggested Agent Prompt

Implement the first symbol-navigation layer. Add dedicated symbol search and exact definition responses for both types and members, keep ordering deterministic, and prove the behavior with unit tests.
