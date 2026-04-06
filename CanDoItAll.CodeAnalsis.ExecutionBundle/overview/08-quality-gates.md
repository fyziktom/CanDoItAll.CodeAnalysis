# Quality gates

## Global definition of done

A subbundle is only done when all of the following are true:

1. relevant build/test commands pass,
2. acceptance criteria in that subbundle are satisfied,
3. diagnostics are explicit rather than swallowed,
4. files are small/cohesive or were split during the refactor pass,
5. public contracts remain transport-agnostic,
6. current CanDoItAll compatibility rules were respected,
7. future `CanDoItAll.Mcp.CodeAnalytics` integration did not become harder,
8. evidence artifacts for the subbundle were updated.

## Hard gates

- **HG-01**: repo + namespace naming map is respected (`CodeAnalsis` root, `CodeAnalytics.*` project family).
- **HG-02**: `.slnx` remains canonical when feasible.
- **HG-03**: no clone of `CanDoItAll.Mcp.Core` is introduced.
- **HG-04**: facts and insights stay separate.
- **HG-05**: the application API remains thin-driver-friendly.
- **HG-06**: final refactor/review pass splits oversized files and removes dumping-ground folders.

## File hygiene policy

Suggested thresholds:
- preferred: under ~250 lines per production file,
- review-needed: above ~350 lines,
- split-required unless strongly justified: above ~450 lines.

The exact automation can be tuned, but the final pass must enforce a real threshold through scripts or tests.

## Architecture gates

- no UI-to-Workspace direct references,
- no Rendering-to-Workspace direct references,
- no Storage-to-UI references,
- no MCP envelope types inside the standalone libraries.

## Review gates

Before closure, run:
- the refactor pass prompt,
- the review pass prompt,
- the full validation matrix from the spreadsheet.
