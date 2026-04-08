# SB-25 Role-aware ranking and selection reasons

## Status

- Completed

## Objective

- Improve infrastructure trouble-path ranking and explain why members and files were selected through strongly typed response metadata.

## Covered Inputs

- implement the improvements you suggest
- infrastructure trouble paths such as `AppDbContext` need role-aware ranking
- the response should explain why members or files were selected

## Prerequisites

- SB-24 passed

## Exact Source References

- C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Application\Services
- C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Abstractions
- C:\repositories\CanDoItAll.CodeAnalsis\tests\CanDoItAll.CodeAnalytics.Tests.Unit

## Deliverables

- Role-aware ranking for infrastructure references
- Strongly typed selection-reason metadata in the response payload
- Tests that prove representative roles and reasons are preserved

## Dependency Impact

- Blocks the lab and rerun because the new metadata must exist before the UI and comparison can judge it.

## Validation Depth

- Focused unit tests
- Build and test pass

## Implementation Steps

1. Model the ranking roles needed for infrastructure seeds such as registration, factory, and schema bootstrap.
2. Thread strongly typed selection-reason metadata through the focused-context response.
3. Update the ranking and excerpt builders to preserve those reasons.
4. Add or update tests that prove the reasons and role-aware ordering.

## Do Not Do

- Do not hide ranking behavior inside anonymous score tweaks.
- Do not add selection reasons as free-form internal strings.

## Acceptance Checklist

- Infrastructure ranking is role-aware instead of arbitrary.
- The response exposes strongly typed selection-reason metadata.
- Tests prove the new ranking or reason behavior.

## Proof Required

- Updated unit tests
- Build and test output
- Bundle execution report updates

## Browser Validation Logging

- N/A in this phase

## Progression Gate

- Later work may continue only when the service can explain selection reasons and infrastructure ranking is measurably more intentional.

## Suggested Agent Prompt

Add role-aware infrastructure ranking and strongly typed selection reasons so the focused-context response can explain why each item was selected before the rerun and UI phases begin.
