# Scenario rubric and method

## Scenarios

The comparison uses three host-repository scenarios chosen to represent materially different navigation shapes:

1. Database scenario: `AppDbContext`
2. Common helper scenario: `IClock`
3. UI scenario: `CanvasSceneHost`

The host repository under analysis is `C:\repositories\CanDoItAll\CanDoItAll.slnx`.

## Goal of the comparison

The study compares the focused-context feature against SharpTools as agent-facing context acquisition mechanisms. The comparison is not trying to prove that one tool replaces the other. It measures the first-pass context quality an agent would realistically get when starting from a symbol or problem area name.

## Execution model

### Focused-context side

- Setup cost is measured as the snapshot build needed before the first query.
- Per-scenario cost is measured as one focused-context query against the already-built snapshot.
- Output is captured through an in-process harness over `ICodeAnalyticsApplicationService` and normalized into a markdown artifact that contains the same categories an agent would consume: seed, intent, precision, implementations, selected types, selected members, usage summary, and file excerpts.

### SharpTools side

- Setup cost is measured as `SharpTool_LoadSolution`.
- Per-scenario cost is measured as a realistic minimal sequence starting from the scenario name only.
- The typical sequence is `SearchDefinitions` followed by the smallest additional tool set needed to get usable first-pass context, such as `ViewDefinition`, `GetMembers`, `ListImplementations`, and `FindReferences`.
- Output is normalized into a markdown artifact that preserves the actual findings returned by the SharpTools calls without carrying JSON wrapper noise that an agent would not intentionally keep.

## Measurements

### Calls

- Setup calls are counted separately from warm per-scenario calls.
- Per-scenario calls count only the calls required after setup is complete.

### Time

- Setup time and per-scenario elapsed time are measured with wall-clock timestamps around the actual tool sequence.
- Time is reported in milliseconds when practical and rounded to seconds in the narrative summary when exact precision is not useful.

### Tokens

- Token counts are estimated consistently for both sides as `ceiling(character_count / 4)`.
- The estimate is intentionally simple. It is used to compare relative context size, not billing precision.

### Noise

Noise is judged manually against the normalized artifacts using this rubric:

- `Low`: most of the content is directly actionable for the next engineering decision.
- `Medium`: useful core context exists, but peripheral material is already visible and requires filtering.
- `High`: the artifact contains enough unrelated or weakly related material that the agent would likely need another filtering pass immediately.

### Helpfulness

Helpfulness is judged manually against the likely next question an engineer or agent would ask:

- `High`: the artifact exposes the next symbol, behavior, or file to inspect without another discovery step.
- `Medium`: the artifact is directionally correct but still needs a clarifying follow-up.
- `Low`: the artifact does not reliably move the investigation forward.

## Success criteria

The comparison is considered useful if it can answer all of the following:

1. Which approach gives the better first-pass context for each scenario?
2. Where does focused-context save calls or tokens without hiding necessary context?
3. Where does SharpTools stay more surgical and why?
4. What concrete product improvements would reduce focused-context noise in the weaker scenarios?
