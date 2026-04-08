# Current state

## Existing baseline

- Focused context now supports typed intent and precision, implementation-aware helper shaping, and usage summaries.
- The lab page can run host-solution scenarios directly through the standalone app.
- SharpTools MCP is available for symbol search, definition viewing, membership inspection, implementation lookup, and reference search.

## Known comparison seeds

- Database seed candidate: `AppDbContext`
- Common helper seed candidate: `IClock`
- UI seed candidate: `CanvasSceneHost`

## Measurement constraints

- Focused-context output is easiest to inspect through the lab UI, so browser proof is part of the workflow.
- SharpTools output arrives as tool payloads, not a rendered page, so the comparison must normalize payload size and usefulness across very different surfaces.
- Exact token counts are not available from the current toolchain, so the study will use one consistent estimation method for both paths.
