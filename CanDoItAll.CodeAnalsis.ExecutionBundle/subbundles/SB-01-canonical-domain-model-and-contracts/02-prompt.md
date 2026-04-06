# Prompt for SB-01 — Canonical domain model and contracts

You are implementing **SB-01** inside the repository `CanDoItAll.CodeAnalsis`.

## Goal
Define the stable request/response contracts and canonical snapshot model that every collector, analyzer, renderer, cache, and future MCP driver will share.

## Read before coding
- overview/04-canonical-snapshot-model.md
- overview/16-compatibility-and-shared-parts.md
- adrs/ADR-003-canonical-snapshot-model.md
- adrs/ADR-005-facts-and-insights-separation.md
- reference/architecture-snapshot-v1.schema.json
- reference/sample-architecture-snapshot.json
- subbundles/SB-01-canonical-domain-model-and-contracts/01-scope.md
- subbundles/SB-01-canonical-domain-model-and-contracts/03-checklist.md
- subbundles/SB-01-canonical-domain-model-and-contracts/04-validation.md
- subbundles/SB-01-canonical-domain-model-and-contracts/05-forbidden-patterns.md
- subbundles/SB-01-canonical-domain-model-and-contracts/06-required-evidence.md

## Also inspect these current CanDoItAll repo files
- src/CanDoItAll.Mcp.Core/Contracts/McpToolEnvelope.cs
- src/CanDoItAll.Mcp.Core/Identity/IdentifierFactories.cs

## Required implementation steps
- Implement `AnalysisRequest`, `BuildSnapshotResult`, progress update contracts, and core snapshot sections.
- Add provenance fields that identify whether data was directly extracted, inferred, or partially resolved.
- Add value objects for normalized solution paths and source locations.
- Write serialization tests for canonical ordering and backwards-compatible defaults.

## Cross-cutting guardrails
- Respect the naming map: repo/solution `CanDoItAll.CodeAnalsis`, project family `CanDoItAll.CodeAnalytics.*`, future driver `CanDoItAll.Mcp.CodeAnalytics`.
- Treat `.slnx` as canonical unless a concrete tool blocker proves otherwise.
- Do not clone `CanDoItAll.Mcp.Core` or any host-repo-only MCP runtime types into the standalone libraries.
- Keep facts, insights, and diagnostics separate.
- Keep comments in English and rare. Avoid XML docs unless a public-contract reason clearly justifies them.
- Keep files small and cohesive. If you temporarily create long files for speed, you must split them before closure.
- Avoid dumping-ground folders such as `Helpers`, `Misc`, or `Stuff`.

## Subbundle-specific stop rules
- Do not continue if any Roslyn symbol types leak into public contracts.
- Do not continue if the snapshot root mixes facts and insights.

## Before you call this subbundle done
- run the listed validation commands,
- update or add the required evidence artifacts,
- verify compatibility with the future `CanDoItAll.Mcp.CodeAnalytics` seam,
- do a local cleanup pass on file size and folder hygiene.
