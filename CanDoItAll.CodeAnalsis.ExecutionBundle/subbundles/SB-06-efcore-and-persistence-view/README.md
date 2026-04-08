# SB-06 EF Core and persistence view

## Status

- Completed

## Objective

- Collect DbContexts, entities, table mappings, and initial persistence diagnostics and expose them through the snapshot and UI.

## Covered Inputs

- EF Core and persistence analysis requirement

## Prerequisites

- `SB-03` completed
- `SB-02` completed

## Exact Source References

- `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Facts\Persistence`
- `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Web\Components\Pages\Snapshots\Persistence.razor`
- `C:\repositories\CanDoItAll.CodeAnalsis\tests\CanDoItAll.CodeAnalytics.Tests.Integration\PersistenceFacts.cs`

## Deliverables

- DbContext facts
- Entity facts
- Initial relationship and mapping diagnostics

## Dependency Impact

- Reopened persistence recovery work in `SB-16` builds directly on this phase.

## Validation Depth

- Build, integration tests, UI smoke, and host validation

## Implementation Steps

1. Discover DbContexts and model hints.
2. Map entities and store objects.
3. Expose persistence results in snapshot and UI.

## Do Not Do

- Do not treat Mermaid or migrations as the primary source of truth.

## Acceptance Checklist

- Persistence page renders.
- Entity and DbContext facts are available in snapshots.

## Proof Required

- Persistence integration tests
- Host validation against CanDoItAll baseline

## Browser Validation Logging

- Logged through the persistence page at baseline only

## Progression Gate

- Persistence baseline exists and can be reopened for stronger relation recovery.

## Suggested Agent Prompt

Collect persistence facts first and keep diagnostics explicit. Avoid hiding missing relationships.
