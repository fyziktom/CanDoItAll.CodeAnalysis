# SB-05 DI and service registration analysis

## Status

- Completed

## Objective

- Collect service registration facts and expose the DI landscape for the standalone snapshot.

## Covered Inputs

- DI and service registration analysis requirement

## Prerequisites

- `SB-03` completed
- `SB-04` completed

## Exact Source References

- `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Facts\Services`
- `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Application`
- `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Web\Components\Pages\Snapshots\Services.razor`

## Deliverables

- Service registration facts
- DI diagnostics
- Service snapshot view

## Dependency Impact

- Later focused context and comparison work can use service registrations as one orientation surface.

## Validation Depth

- Build, unit tests, and UI smoke

## Implementation Steps

1. Parse service registrations from source.
2. Normalize lifetime and implementation data.
3. Expose results through application and UI views.

## Do Not Do

- Do not hide unresolved registrations silently.

## Acceptance Checklist

- Service registrations are queryable.
- Ambiguities are surfaced as diagnostics.

## Proof Required

- Unit tests for DI collection
- Host snapshot service counts

## Browser Validation Logging

- Logged through the snapshot services page at baseline only

## Progression Gate

- Service facts are stable enough for later orientation comparisons.

## Suggested Agent Prompt

Collect DI data explicitly and surface ambiguities instead of inventing fallback truth.
