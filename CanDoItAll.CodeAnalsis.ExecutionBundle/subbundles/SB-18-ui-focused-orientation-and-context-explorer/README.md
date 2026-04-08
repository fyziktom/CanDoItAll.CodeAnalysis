# SB-18 UI focused orientation and context explorer

## Status

- Completed

## Objective

- Expose the new focused context query and scoped diagrams in the SSR UI so the value can be tested through real user flow.

## Covered Inputs

- Search through types in a project
- Ask for functions inside classes
- Surface focused context instead of forcing whole-file reading

## Prerequisites

- `SB-16` completed
- `SB-17` completed
- Baseline `SB-11` and `SB-12` remain trusted

## Exact Source References

- `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Web`
- `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Application`
- `C:\repositories\CanDoItAll.CodeAnalsis\tests\CanDoItAll.CodeAnalytics.Tests.Web`
- `C:\repositories\CanDoItAll.CodeAnalsis\output\playwright`

## Deliverables

- UI route or panel for focused context exploration
- Better scoped diagram access from the UI
- Clear source-reference display for selective deeper reading

## Dependency Impact

- Final comparison and closure depend on a real UI flow that demonstrates the value.

## Validation Depth

- Build, web tests, Playwright proof, screenshot review, and host-route smoke

## Implementation Steps

1. Add focused context entry points to the UI.
2. Render related members, types, and source references clearly.
3. Validate the flow in a real browser.

## Do Not Do

- Do not dump raw JSON into the UI as the primary experience.
- Do not hide query limits or error states.

## Acceptance Checklist

- A user can start from a type or member and inspect bounded related context.
- Source references are visible and readable.
- Scoped diagrams are reachable from the same exploration flow or nearby.

## Proof Required

- Web tests
- Playwright route proof and screenshots
- Manual screenshot review notes in execution report

## Browser Validation Logging

- Required for the focused context route and the updated snapshot routes.

## Progression Gate

- Final comparison may continue only after the UI proves the new context flow is practically usable.

## Suggested Agent Prompt

Expose focused context through a clean SSR flow that helps orientation without overwhelming the screen.
