# SB-15 Refactor foundation and canonical ownership

## Status

- Completed

## Objective

- Perform the refactor-first pass that reduces oversized files and re-establishes clear sources of truth before adding new feature breadth.

## Covered Inputs

- Refactor first
- Analyze whole solution and find architecture gaps
- Focus on long files, sources of truth, and isolation of helpers

## Prerequisites

- `SB-14` completed baseline remains trusted
- Bundle repair completed and readiness gate passed

## Exact Source References

- `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Facts\Persistence`
- `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Facts\Dependencies`
- `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Application\Services`
- `C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.ExecutionBundle\analysis\01-current-state.md`

## Deliverables

- Smaller collector and orchestrator ownership slices
- Updated file-length posture
- Explicit canonical ownership for new work

## Dependency Impact

- Blocks every later reopened phase because adding features on top of muddled ownership would make the repo harder to transplant and harder to trust.

## Validation Depth

- Full build and test run plus hotspot review

## Implementation Steps

1. Split oversized collectors and orchestrators by responsibility.
2. Keep source-of-truth placement explicit.
3. Update tests if refactor changes internal ownership boundaries.

## Do Not Do

- Do not widen feature scope during the refactor pass.
- Do not hide logic in generic `Helpers` folders.

## Acceptance Checklist

- Primary hotspots are reduced or made clearly bounded.
- Ownership boundaries are easier to explain in one sentence each.
- Build and tests still pass.

## Proof Required

- `dotnet build .\CanDoItAll.CodeAnalsis.slnx -warnaserror`
- `dotnet test .\CanDoItAll.CodeAnalsis.slnx --no-build`
- Updated hotspot inventory or refactor report

## Browser Validation Logging

- N/A

## Progression Gate

- Later work may continue only after refactor proof shows the new features will land on cleaner ownership seams.

## Suggested Agent Prompt

Refactor only. Split collectors and orchestrators along canonical ownership lines and keep behavior stable.
