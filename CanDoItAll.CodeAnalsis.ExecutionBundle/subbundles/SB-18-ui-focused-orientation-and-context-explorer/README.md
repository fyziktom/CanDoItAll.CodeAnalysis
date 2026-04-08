# SB-18 UI focused orientation and context explorer

## Status

- Completed

## Objective

- Expose the reopened focused-context query through a dedicated SSR tuning page where workspace scope, prompt text, tags, and accordion-based file excerpts can be judged together.

## Covered Inputs

- Search through types in a project
- Ask for functions inside classes
- Surface focused context instead of forcing whole-file reading
- Select a solution plus optional project scope from the tuning page
- Enter prompt text and tags directly instead of deep-linking through other pages
- Review grouped excerpts, file accordions, and line stats below the form

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

- Dedicated focused-context lab route
- Workspace, project, prompt, depth, and tag controls on the same page
- Accordion-based file excerpts with stats and clear source-reference display
- Better nearby access to focused context and scoped diagrams from the existing snapshot flow

## Dependency Impact

- Final comparison and closure depend on a real UI flow that demonstrates the value and supports heuristic tuning feedback.

## Validation Depth

- Build, web tests, Playwright proof, screenshot review, and host-route smoke

## Implementation Steps

1. Add a dedicated focused-context lab entry point to the UI.
2. Render related members, types, grouped excerpts, and stats clearly.
3. Keep the existing snapshot drilldown paths wired into the same focused-context surface.
4. Validate the flow in a real browser.

## Do Not Do

- Do not dump raw JSON into the UI as the primary experience.
- Do not hide query limits or error states.

## Acceptance Checklist

- A user can select solution or project scope, enter prompt text, add tags, and inspect bounded related context.
- Source references, excerpts, and line stats are visible and readable.
- The output is grouped by file with usable accordions.
- Scoped diagrams remain reachable from the same exploration flow or nearby.

## Proof Required

- Web tests
- Playwright route proof and screenshots
- Manual screenshot review notes in execution report
- Evidence that the page makes tuning feedback possible instead of hiding the scoring choices

## Browser Validation Logging

- Required for the dedicated lab route and the updated snapshot routes.

## Progression Gate

- Final comparison may continue only after the UI proves the new context flow is practically usable for tuning, not only for one deep-linked happy path.

## Suggested Agent Prompt

Expose focused context through a clean SSR lab flow that helps orientation without overwhelming the screen and makes excerpt precision easy to judge.
