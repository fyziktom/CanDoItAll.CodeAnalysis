# Target solution

## Study shape

- One standalone comparison bundle owns scenario choice, measurement rubric, execution evidence, and final analysis.
- Focused-context evidence is gathered through the standalone app against the host solution.
- SharpTools evidence is gathered directly from the MCP on the same host solution.

## Measurement model

- Setup cost is tracked separately:
  - focused context: snapshot or app warm-up context
  - SharpTools: `LoadSolution`
- Per-scenario comparison is warm:
  - one focused-context query per scenario
  - one SharpTools sequence per scenario
- Output size is normalized into:
  - file count or symbol count
  - excerpt or payload text length
  - estimated tokens using a shared heuristic
- Usefulness and noise are judged against the same questions:
  - does this answer the likely next engineering question?
  - how much unrelated content is mixed in?
  - how quickly does it expose the next exact symbol to inspect?
