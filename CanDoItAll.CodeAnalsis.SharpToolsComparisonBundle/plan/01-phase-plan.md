# Phase plan

## Execution Order

1. Validate the comparison bundle structure and freeze the rubric.
2. Run focused-context evidence for the three chosen scenarios.
3. Run SharpTools evidence for the same scenarios.
4. Normalize metrics and write the comparative findings.
5. Close the bundle with updated tables and final validation.

## Subbundle Dependency Map

```mermaid
flowchart TD
    SB00["SB-00 Scenario Selection And Rubric"]
    SB01["SB-01 Focused Context Scenario Runs"]
    SB02["SB-02 SharpTools Scenario Runs"]
    SB03["SB-03 Comparative Analysis And Closure"]

    SB00 --> SB01
    SB00 --> SB02
    SB01 --> SB03
    SB02 --> SB03
```

## Critical Subbundles

- `SB-00` is critical because scenario drift or a weak rubric makes every later comparison less trustworthy.
- `SB-03` is critical because it owns the normalized conclusion instead of raw evidence fragments only.

## Phase Gates

| Phase | Entry gate | Closure gate |
| --- | --- | --- |
| `SB-00` | Raw request and measurement goal are understood | Three scenarios and one comparison rubric are frozen |
| `SB-01` | Scenario list is frozen | Focused-context outputs, timings, and browser proof are captured |
| `SB-02` | Scenario list is frozen | SharpTools outputs, timings, and call counts are captured |
| `SB-03` | Both evidence subbundles are complete | Final comparison tables, note closure, and completed validation all pass |
