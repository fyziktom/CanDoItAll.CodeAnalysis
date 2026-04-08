# SB-12 UI drilldown search and export

## Status

- Completed

## Objective

- Add snapshot drilldown pages, search, export access, and project or project-file selection support.

## Covered Inputs

- UI drilldown, search, and export requirements

## Prerequisites

- `SB-11` completed
- `SB-10` completed

## Exact Source References

- `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Web\Components\Pages\Snapshots`
- `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Web\Operations`
- `C:\repositories\CanDoItAll.CodeAnalsis\output\playwright`

## Deliverables

- Snapshot detail pages
- Type search
- Export links
- Workspace selection flow

## Dependency Impact

- Reopened focused-orientation UI work in `SB-18` extends these drilldown surfaces.

## Validation Depth

- Build, web tests, and Playwright proof

## Implementation Steps

1. Add snapshot detail navigation.
2. Expose search and export flows.
3. Surface progress and errors through the UI.

## Do Not Do

- Do not let the UI invent analysis data that the application layer does not provide.

## Acceptance Checklist

- Users can navigate snapshot drilldowns.
- Search and exports are reachable.

## Proof Required

- Browser proof of drilldown and export routes
- Web UI tests

## Browser Validation Logging

- Logged through snapshot detail and exports routes at baseline only

## Progression Gate

- Drilldown UI is stable enough for later focused context exploration.

## Suggested Agent Prompt

Extend the SSR drilldowns without leaking business logic into components.
