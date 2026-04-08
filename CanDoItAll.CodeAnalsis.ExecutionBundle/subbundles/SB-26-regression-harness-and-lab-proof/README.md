# SB-26 Regression harness and lab proof

## Status

- Completed

## Objective

- Make the comparison rerun repeatable and surface the new response-shaping metadata in the focused-context lab.

## Covered Inputs

- implement the improvements you suggest
- revalidate how we stand after they are implemented
- the comparison should be rerunnable without rebuilding the methodology from scratch

## Prerequisites

- SB-24 passed
- SB-25 passed

## Exact Source References

- C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Web
- C:\repositories\CanDoItAll.CodeAnalsis\tests
- C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.SharpToolsComparisonBundle

## Deliverables

- Repeatable rerun harness or equivalent checked-in validation path
- Lab UI surfaces the new precision and reason metadata clearly
- Browser proof for the changed UI

## Dependency Impact

- Blocks final validation because the rerun must be repeatable and the UI must prove the new output shape.

## Validation Depth

- Web tests
- Playwright proof
- Build and test pass

## Implementation Steps

1. Turn the comparison rerun path into a repeatable checked-in harness or equivalent validation flow.
2. Surface the new precision and selection-reason metadata in the lab UI.
3. Update web tests to cover the new visible response shape.
4. Capture Playwright proof for the changed UI.

## Do Not Do

- Do not leave the rerun methodology trapped in an ad hoc local-only script.
- Do not change the lab UI without real browser proof.

## Acceptance Checklist

- The rerun path is repeatable from the repo.
- The lab UI shows the new metadata clearly.
- Web tests and browser proof cover the UI changes.

## Proof Required

- Updated web tests
- Playwright proof
- Bundle execution report updates

## Browser Validation Logging

- Required for the changed lab route

## Progression Gate

- Final rerun may continue only when the harness and lab can expose the new response shape repeatably.

## Suggested Agent Prompt

Make the rerun path repeatable and update the focused-context lab to show the new precision and reason metadata, then prove the UI changes with tests and Playwright.
