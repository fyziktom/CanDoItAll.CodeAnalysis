# SB-29 Twenty-scenario real-world evaluation and tuning

## Status

- Completed

Completion date: 2026-05-09.

## Objective

Create and run a repeatable evaluation suite with at least 20 real-world-looking simulated agent prompts across read-only target repositories, score CodeAnalytics output quality, implement the smallest justified tuning improvements, and rerun the same scenarios to quantify whether the tool actually improved.

## Covered Inputs

- `inputs/08-twenty-scenario-real-world-evaluation.md`

## Prerequisites

- SB-24 through SB-28 remain trusted.
- Focused context supports tags, relation hints, depth, intent, precision, selection reasons, and metric output.
- External target repositories must remain read-only.

## Exact Source References

- `C:\repositories\CanDoItAll.CodeAnalsis\tools`
- `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Application\Services`
- `C:\repositories\CanDoItAll.CodeAnalsis\tests`
- `C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.ExecutionBundle\analysis`

## Deliverables

- A repeatable scenario-evaluation harness or extension that runs at least 20 simulated prompts.
- Scenario records that include the simulated user prompt, intended agent approach, repository, scope, query text, tags, relation hints, depth, intent, and precision.
- Baseline artifacts with raw focused-context output and scored usefulness/noise metrics.
- A written analysis identifying concrete improvement opportunities.
- A minimal implementation pass driven by the baseline findings.
- After-change rerun artifacts using the same scenarios.
- Before/after comparison summary with quantified changes.

## Dependency Impact

- This subbundle tests the core context-saving claim across multiple codebases. If results are poor, future MCP skill guidance should be tightened instead of encouraging broad focused-context usage.
- The harness becomes a regression tool for later tuning work.

## Validation Depth

- Build the scenario harness.
- Run baseline scenarios against read-only repos.
- Implement only CodeAnalytics repo changes.
- Rerun the same scenarios after changes.
- Run standalone build and focused tests.
- Run completed-stage bundle validation.

## Implementation Steps

1. Define at least 20 simulated prompts, including at least 5 project-introduction scenarios.
2. Implement a repeatable evaluator that records raw focused-context outputs and scoring metrics.
3. Run baseline evaluation into bundle analysis artifacts.
4. Analyze baseline output and select the smallest improvement set.
5. Implement improvements in the CodeAnalytics repo only.
6. Rerun the scenario suite and compare before/after metrics.
7. Record closure evidence in the bundle.

## Scope Exceptions

- Do not modify `C:\repositories\MBusParser`, `C:\repositories\influxdb-client-csharp`, or `C:\repositories\CanDoItAll`.
- Do not implement a full semantic relevance oracle. The first scoring pass can use transparent heuristic scoring plus scenario expectations.
- Do not add broad UI work unless the evaluation proves the lab itself blocks testing.

## Do Not Do

- Do not change source files in external target repositories.
- Do not select only scenarios that already make CodeAnalytics look good.
- Do not count lower token output as improvement if the expected relevant files or symbols disappear.
- Do not implement broad ranking rewrites before baseline evidence identifies a specific failure mode.

## Acceptance Checklist

- At least 20 scenarios run successfully.
- At least 5 scenarios are first-step introduction/orientation prompts.
- Every scenario records a simulated prompt and the agent's intended CodeAnalytics approach.
- Baseline and after-change runs include file, block, line, character, token, cluster, caller, elapsed, helpfulness, noise, and missing-context style metrics.
- Bundle analysis states which changes improved results and which gaps remain.
- External target repositories have no modified files caused by this work.

## Proof Required

- Baseline run artifacts are under `analysis\twenty-scenario-evaluation\baseline`.
- After-change run artifacts are under `analysis\twenty-scenario-evaluation\after`.
- Before/after summary is under `analysis\twenty-scenario-evaluation`.
- Standalone build/test proof passed.
- Completed-stage bundle validation passed.

## Browser Validation Logging

- Browser proof is not required for SB-29 unless UI changes are introduced.
- If UI changes become necessary, validate `/context-lab` at desktop and mobile widths and record screenshots in `output\playwright`.

## Progression Gate

- Passed. The same 22 scenarios have before/after results. Average helpfulness improved from 0.434 to 0.714, failed scenarios dropped from 9 to 0, and no scenario regressed.

## Suggested Agent Prompt

Run the 20+ scenario evaluation harness against read-only target repositories, score baseline CodeAnalytics focused-context outputs, implement the smallest measured engine or harness improvements, rerun the exact same scenarios, and record before/after evidence in the bundle without modifying external repositories.
