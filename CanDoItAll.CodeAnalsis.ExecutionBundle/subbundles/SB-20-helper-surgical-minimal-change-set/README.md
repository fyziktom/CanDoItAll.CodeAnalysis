# SB-20 Helper surgical minimal change set

## Status

- Completed

## Objective

- Land the smallest strongly typed helper-precision foundation so high-fan-in helper queries stop using the same traversal shape as trouble-path queries.

## Covered Inputs

- Make helpers like `IClock` more surgical and precise
- Start with the minimal change set first
- Reduce helper noise where necessary
- Keep the result closer to SharpTools precision without pretending to replace SharpTools

## Prerequisites

- `SB-00` through `SB-19` remain trusted
- Helper-noise findings are modeled explicitly in the repaired bundle

## Exact Source References

- `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Application`
- `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Abstractions`
- `C:\repositories\CanDoItAll.CodeAnalsis\tests\CanDoItAll.CodeAnalytics.Tests.Unit`

## Deliverables

- Explicit focused-context helper intent or precision model
- Helper-seed classification based on exact-symbol shape and broad consumer spread
- Directional traversal for helper mode instead of unconditional undirected expansion
- Regression tests for helper-oriented seed and traversal behavior

## Dependency Impact

- Blocks all later helper-mode work because wider response shaping is unsafe before the minimal strategy boundary exists.

## Validation Depth

- Build, unit tests, focused host rerun, and proof that database or UI defaults did not regress

## Implementation Steps

1. Add the minimum typed strategy boundary for helper-like focused-context requests.
2. Detect high-fan-in helper seeds such as `IClock`.
3. Switch helper mode to a more surgical traversal path with stricter stop rules.
4. Validate that the previous database and UI good cases still behave acceptably.

## Do Not Do

- Do not add broad UI or payload redesign in this phase.
- Do not chase SharpTools parity through more score tweaks alone.

## Acceptance Checklist

- Helper seeds no longer rely on the same default traversal used for trouble-path queries.
- `IClock`-style queries are materially narrower than the current baseline.
- The result is strongly typed and explainable in code.
- Existing database and UI flows still work.

## Proof Required

- `dotnet build .\CanDoItAll.CodeAnalsis.slnx -nologo`
- Focused unit tests for helper classification and traversal
- Host helper rerun note against the existing baseline

## Browser Validation Logging

- N/A in this phase

## Progression Gate

- The workflow may continue only when helper-mode routing is real, tested, and not just implied by new scoring constants.

## Suggested Agent Prompt

Implement the minimal helper-precision change set: classify helper seeds, add a surgical traversal mode, and prove that it narrows `IClock`-style outputs without regressing the stronger database and UI cases.
