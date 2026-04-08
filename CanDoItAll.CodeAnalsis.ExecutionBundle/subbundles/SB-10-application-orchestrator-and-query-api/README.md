# SB-10 Application orchestrator and query API

## Status

- Completed

## Objective

- Orchestrate collection, insight assembly, persistence, and query responses through the application layer.

## Covered Inputs

- Application orchestrator and query API requirement

## Prerequisites

- `SB-08` completed
- `SB-09` completed

## Exact Source References

- `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Application`
- `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Abstractions`
- `C:\repositories\CanDoItAll.CodeAnalsis\tests\CanDoItAll.CodeAnalytics.Tests.Unit\ApplicationFacts.cs`

## Deliverables

- Snapshot build orchestration
- Query responses for dashboard and detail pages
- Progress reporting hooks

## Dependency Impact

- Reopened focused-context work in `SB-17` extends this application surface.

## Validation Depth

- Build, unit tests, and integration assertions

## Implementation Steps

1. Orchestrate workspace loading, fact collection, insights, and exports.
2. Persist snapshots and expose query responses.
3. Keep transport concerns out of the application layer.

## Do Not Do

- Do not place web routing or component logic in the application project.

## Acceptance Checklist

- Snapshot builds complete through one application entry point.
- Query responses remain transport-agnostic.

## Proof Required

- Application unit tests
- Integration tests for snapshot builds

## Browser Validation Logging

- N/A

## Progression Gate

- Application service is stable enough for UI and future MCP seams.

## Suggested Agent Prompt

Keep orchestration in the application layer and leave transport-specific concerns out.
