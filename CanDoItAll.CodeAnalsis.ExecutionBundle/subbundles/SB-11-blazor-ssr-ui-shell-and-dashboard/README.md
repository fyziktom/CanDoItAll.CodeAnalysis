# SB-11 Blazor SSR UI shell and dashboard

## Status

- Completed

## Objective

- Ship the first SSR-first web shell and dashboard for snapshot execution and overview.

## Covered Inputs

- SSR-first UI requirement

## Prerequisites

- `SB-10` completed

## Exact Source References

- `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Web`
- `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Web\Components\Pages`
- `C:\repositories\CanDoItAll.CodeAnalsis\tests\CanDoItAll.CodeAnalytics.Tests.Web\WebUiFacts.cs`

## Deliverables

- Home page
- Dashboard page
- Operation tracking shell

## Dependency Impact

- Reopened UI exploration work in `SB-18` builds directly on this SSR shell.

## Validation Depth

- Build, web tests, and Playwright proof

## Implementation Steps

1. Build the SSR shell and navigation.
2. Surface dashboard and operation state.
3. Keep component logic thin.

## Do Not Do

- Do not move collection logic into components.

## Acceptance Checklist

- The web shell starts and renders.
- Dashboard and operation views work against persisted snapshots.

## Proof Required

- Web tests
- Browser proof of the dashboard baseline

## Browser Validation Logging

- Logged through the dashboard route at baseline only

## Progression Gate

- UI shell is stable enough for deeper context exploration.

## Suggested Agent Prompt

Use the application layer from SSR components and keep state transitions explicit.
