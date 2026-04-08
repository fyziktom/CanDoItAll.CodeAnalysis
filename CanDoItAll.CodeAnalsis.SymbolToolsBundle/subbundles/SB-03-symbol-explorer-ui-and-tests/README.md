# SB-03 Symbol explorer UI and tests

## Status

- Completed

## Objective

- Add one snapshot route where search, definition, members, implementations, and references can be exercised together.

## Covered Inputs

- both ways how agent can reach informations

## Exact Source References

- C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Web\Components\Pages\Snapshots
- C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Web\Components\Shared\SnapshotTabs.razor
- C:\repositories\CanDoItAll.CodeAnalsis\tests\CanDoItAll.CodeAnalytics.Tests.Web\WebUiFacts.cs

## Prerequisites

- SB-01 and SB-02 are complete and trusted.

## Deliverables

- New snapshot UI route
- Snapshot tab integration
- Web tests
- Real browser proof with screenshots

## Dependency Impact

- Provides the second interactive path the user asked for and is the only phase with required browser analytics.

## Validation Depth

- Web tests
- Playwright route proof

## Implementation Steps

1. Add a symbol explorer route and wire it into snapshot navigation.
2. Render search results, definition, members, implementations, and references on one page.
3. Add or update web tests.
4. Capture Playwright proof and screenshots.

## Do Not Do

- Do not duplicate the types page without a clearer inspection workflow.
- Do not close this phase without real browser proof.

## Acceptance Checklist

- The route is reachable from snapshot tabs.
- Search and selected symbol details both render.
- Web tests pass.
- Browser proof is recorded with screenshots.

## Proof Required

- Passing web tests
- Playwright analytics rows and screenshots

## Browser Validation Logging

- Required in this phase

## Progression Gate

- Comparison rerun may continue only when the UI route is stable and browser proof is recorded.

## Suggested Agent Prompt

Add a usable symbol explorer page. It should let the user search a symbol, inspect the definition, list members, inspect implementations, and review references without leaving the snapshot workflow, and it must be proven in Playwright.
