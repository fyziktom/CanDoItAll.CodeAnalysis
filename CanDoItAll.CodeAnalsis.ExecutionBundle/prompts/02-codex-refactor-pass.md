# Refactor pass prompt

Run this after the main implementation slice is complete.

## Goals

- split oversized files,
- remove dumping-ground folders,
- clarify names,
- restore clear boundaries,
- simplify any overgrown methods/classes,
- ensure the root naming split is still respected (`CodeAnalsis` vs `CodeAnalytics`).

## Mandatory checks

- Are any production files above the agreed file-length threshold?
- Did any project start depending on layers it should not see?
- Did any UI code absorb application or collector logic?
- Did any host-repo MCP concerns leak into the standalone libraries?
- Did any ad-hoc helper folders appear?
- Are comments still English-only and rare?
- Are there XML docs that can be removed because they are noise rather than contract help?

## Output

Produce a concise refactor report:
- files split,
- files renamed,
- folders cleaned,
- remaining justified exceptions.
