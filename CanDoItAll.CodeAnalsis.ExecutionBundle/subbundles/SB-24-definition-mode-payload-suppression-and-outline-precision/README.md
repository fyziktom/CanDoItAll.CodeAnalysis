# SB-24 Definition mode payload suppression and outline precision

## Status

- Completed

## Objective

- Reduce helper definition-mode payload cost and add a lighter outline-style precision mode without regressing the strong UI and database cases.

## Covered Inputs

- implement the improvements you suggest
- helper definition mode still carries too much consumer code
- usage summary should not force large consumer excerpts into the main payload
- a lighter outline-style precision mode is needed

## Prerequisites

- SB-00 through SB-23 remain trusted
- The comparison-driven reopen is documented in the repaired bundle

## Exact Source References

- C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Application\Services
- C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Abstractions
- C:\repositories\CanDoItAll.CodeAnalsis\tests\CanDoItAll.CodeAnalytics.Tests.Unit

## Deliverables

- Helper definition mode suppresses large consumer excerpts by default
- Usage-summary shaping is separated more aggressively from excerpt selection
- Outline-style precision is available through the focused-context query contract
- Unit coverage proves the new payload boundaries

## Dependency Impact

- Blocks later ranking, UI, and rerun work because the payload contract changes here.

## Validation Depth

- Focused unit tests
- Build and test pass

## Implementation Steps

1. Add the lighter precision mode to the abstractions and query flow.
2. Suppress helper consumer excerpts more aggressively in helper definition mode.
3. Keep usage-summary breadth available without forcing the same consumers into the main excerpt list.
4. Add or update unit tests around the new payload contract.

## Do Not Do

- Do not regress the existing UI and database cases while tightening helper mode.
- Do not add another loose string-based precision toggle.

## Acceptance Checklist

- Helper definition mode is materially tighter by default.
- Outline-style precision exists in the query contract.
- Usage summary can remain visible without dragging large consumer excerpts into the main file set.
- Unit tests cover the new shaping behavior.

## Proof Required

- Updated unit tests
- Build and test output
- Bundle execution report updates

## Browser Validation Logging

- N/A in this phase

## Progression Gate

- Later work may continue only when helper definition mode is materially tighter and the new precision mode exists.

## Suggested Agent Prompt

Tighten helper definition-mode payload shaping first, add the lighter precision option, and prove the new response boundary with focused tests before touching ranking or UI work.
