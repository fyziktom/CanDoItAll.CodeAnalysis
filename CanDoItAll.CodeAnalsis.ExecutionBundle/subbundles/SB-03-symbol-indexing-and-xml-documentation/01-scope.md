# SB-03 — Symbol indexing and XML documentation ingestion

## Objective
Collect symbol-level facts such as namespaces, types, members, inheritance, source locations, and XML documentation summaries from the analyzed solution.

## Milestone / priority / actor
- Milestone: `M2`
- Priority: `P0`
- Primary actor: `Facts maintainer`

## Depends on
- SB-01
- SB-02

## Read first
- overview/05-analysis-pipeline.md
- overview/04-canonical-snapshot-model.md
- adrs/ADR-002-roslyn-first-analysis.md

## Current CanDoItAll reference files to inspect
- .github/copilot-instructions.md

## In scope
- Enumerate namespaces, named types, base types, implemented interfaces, key members, and source locations.
- Resolve and normalize XML documentation summaries from the analyzed projects where available.
- Persist unresolved or ambiguous symbol states as diagnostics instead of hiding them.
- Index symbols in a deterministic, query-friendly shape.

## Out of scope
- No DI heuristics yet.
- No persistence heuristics yet.
- No UI-specific search model yet.

## Compatibility rules specific to this subbundle
- The analyzed target code may contain XML documentation even though the new repo itself should avoid XML-doc sprawl.
- Keep symbol fact records plain so the future MCP driver can summarize them without Roslyn references.

## Expected deliverables
- Type/member fact collectors.
- XML documentation resolver and normalizer.
- Unit/integration tests around generics, partial types, and missing docs.
