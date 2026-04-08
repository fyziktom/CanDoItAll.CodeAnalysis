# SB-00 Scenario selection and rubric

## Status

- Completed

## Objective

- Freeze the three scenarios and the exact comparison rubric before any evidence collection starts.

## Covered Inputs

- select 3 different scenarios for test
- Analyze the outputs if they are trully helpful
- how much noise they contains
- how many tokens do it takes
- how many calls
- how much time

## Prerequisites

- The raw request is normalized into this bundle
- The host solution path is confirmed

## Exact Source References

- C:\repositories\CanDoItAll\CanDoItAll.slnx
- C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.SharpToolsComparisonBundle

## Deliverables

- Three named scenarios
- One explicit usefulness and noise rubric
- One explicit token, call, and timing measurement method

## Dependency Impact

- Blocks all evidence gathering because later phases depend on fair scenario and measurement choices

## Validation Depth

- Bundle doc review
- Scenario justification review

## Implementation Steps

1. Choose three scenarios that cover database, helper, and UI-oriented navigation.
2. Define how usefulness and noise will be judged.
3. Define a consistent token estimate method and time-capture method.

## Do Not Do

- Do not start collecting outputs before the rubric is frozen.
- Do not pick three variants of the same kind of scenario.

## Acceptance Checklist

- The scenario list is explicit.
- The comparison rubric is explicit.
- The token, call, and timing method is explicit.

## Proof Required

- Updated execution report with the frozen scenario list
- Updated analysis notes with the rubric

## Browser Validation Logging

- N/A in this phase

## Progression Gate

- Later phases may continue only when the scenario list and rubric are frozen and documented.

## Closure Notes

- Completed on 2026-04-08.
- Proof is stored in `analysis/03-scenario-rubric-and-method.md`.

## Suggested Agent Prompt

Freeze the comparison design first. Choose three host-solution scenarios and one explicit measurement rubric before gathering any focused-context or SharpTools evidence.
