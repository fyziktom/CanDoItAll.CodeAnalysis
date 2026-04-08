# Assumptions and risks

## Working Assumptions

- Three scenarios are enough to expose meaningful differences if they span database, helper, and UI navigation.
- Warm per-scenario comparisons are more representative than folding one-time setup cost into every run.
- An estimated token method based on carried text length is acceptable for relative comparison.

## Critical Path Risks

- The host solution is large enough that SharpTools discovery or focused-context snapshot reuse may vary run to run.
- The managed app health probe may remain flaky even when the page itself is usable.
- Some SharpTools sequences may need an extra discovery call if symbol names are ambiguous.

## Validation Risks

- Browser timings will reflect the rendered route and not only the underlying focused-context query.
- SharpTools elapsed time will be wall-clock across the actual MCP sequence, not an internal server-side stopwatch.
- Usefulness and noise still need human judgment even when line counts and tokens look good.

## Reopen Triggers

- Any scenario that cannot be resolved into a fair SharpTools sequence should reopen `SB-00`.
- Missing browser proof for focused-context scenarios should reopen `SB-01`.
- Missing call-count or elapsed-time capture for either side should reopen the affected subbundle.
