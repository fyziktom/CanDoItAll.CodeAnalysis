# SB-22 Helper precision response shaping and UI

## Status

- Completed

## Objective

- Add the broader helper-mode improvements so surgical helper exploration returns definitions, implementations, and sampled or summarized usages in a UI that makes noise visible and breadth understandable.

## Covered Inputs

- Add the broader `How To Improve It` points
- Prefer definitions, implementations, and sampled usages over indiscriminate consumer spread
- Group or summarize widespread helper consumers instead of loading all of them into the main excerpt payload
- Make the result understandable in the lab UI

## Prerequisites

- `SB-21` passed
- `SB-16` remains trusted

## Exact Source References

- `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Abstractions`
- `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Application`
- `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Web`
- `C:\repositories\CanDoItAll.CodeAnalsis\tests\CanDoItAll.CodeAnalytics.Tests.Unit`
- `C:\repositories\CanDoItAll.CodeAnalsis\tests\CanDoItAll.CodeAnalytics.Tests.Web`

## Deliverables

- Helper-mode payload additions for implementations and sampled or summarized usages
- Diversity-aware usage sampling instead of naive consumer accumulation
- Lab UI support for helper breadth and omitted-consumer summaries
- Web and unit coverage for the new helper presentation

## Dependency Impact

- Final validation depends on a user-facing helper result that is understandable enough to compare fairly against SharpTools.

## Validation Depth

- Build, tests, Playwright proof, host rerun, and screenshot review

## Implementation Steps

1. Shape helper-mode responses around contract, implementations, and sampled usages.
2. Add diversity-aware consumer sampling or grouping.
3. Surface helper breadth and omitted-count information in the lab UI.
4. Validate the helper-mode UI flow in a real browser.

## Do Not Do

- Do not dump every consumer into the main file accordion set.
- Do not regress the stronger database and UI trouble-path surfaces.

## Acceptance Checklist

- Helper outputs are visibly more surgical than the current baseline.
- Implementation types and representative consumers are clear.
- The UI explains breadth instead of hiding it inside raw file count only.
- Database and UI cases still remain useful.

## Proof Required

- `dotnet build .\CanDoItAll.CodeAnalsis.slnx -nologo`
- `dotnet test .\CanDoItAll.CodeAnalsis.slnx -nologo`
- Playwright proof for helper-mode output on the lab page
- Host rerun note showing narrower helper results than the previous baseline

## Browser Validation Logging

- Required for the helper-oriented lab view and any new summary elements.

## Progression Gate

- Final comparison may continue only after the helper-mode result is understandable enough to judge usefulness and noise honestly.

## Suggested Agent Prompt

Implement the broader helper-mode improvements after the strategy and refactor passes are stable. Keep the output surgical by emphasizing implementations and sampled usages instead of full consumer spread.
