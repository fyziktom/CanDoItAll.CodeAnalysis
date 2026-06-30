# SB05 Semantic Invariants

## Preserved Behavior

- Snapshots still store and reload through `FileSnapshotRepository`.
- Markdown, Mermaid, and JSON serialization golden tests still pass.
- Symbol search still supports regex mode for valid user patterns.
- Document, symbol, and focused-context source responses keep the same public response contracts.

## Strengthened Behavior

- Snapshot exports cannot be written outside the intended snapshot directory.
- User-provided regex symbol search no longer pays dynamic compiled-regex startup cost and has a 100 ms match timeout.
- A timed-out regex fails closed for the rest of that request instead of throwing or repeatedly timing out.
- Source file reads for public document/excerpt APIs are limited to files under the workspace root and at most 2 MB.

## Deferred Work

- Rendering/query LINQ and list allocations remain intentionally unchanged until a scenario or benchmark proves they are bottlenecks.
- File-length guardrail closure remains assigned to SB06 and final cleanup.
