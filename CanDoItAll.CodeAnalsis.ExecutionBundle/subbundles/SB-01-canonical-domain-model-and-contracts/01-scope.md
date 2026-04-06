# SB-01 — Canonical domain model and contracts

## Objective
Define the stable request/response contracts and canonical snapshot model that every collector, analyzer, renderer, cache, and future MCP driver will share.

## Milestone / priority / actor
- Milestone: `M1`
- Priority: `P0`
- Primary actor: `Domain maintainer`

## Depends on
- SB-00
- SB-00A

## Read first
- overview/04-canonical-snapshot-model.md
- overview/16-compatibility-and-shared-parts.md
- adrs/ADR-003-canonical-snapshot-model.md
- adrs/ADR-005-facts-and-insights-separation.md
- reference/architecture-snapshot-v1.schema.json
- reference/sample-architecture-snapshot.json

## Current CanDoItAll reference files to inspect
- src/CanDoItAll.Mcp.Core/Contracts/McpToolEnvelope.cs
- src/CanDoItAll.Mcp.Core/Identity/IdentifierFactories.cs

## In scope
- Create immutable request, response, progress, diagnostics, and snapshot model types.
- Define stable identifiers and source references for solutions, projects, documents, symbols, services, entities, and findings.
- Separate facts, insights, diagnostics, and exports at the top level.
- Guarantee deterministic JSON ordering and round-trippable serialization.

## Out of scope
- No Roslyn-specific symbols leaking into contracts.
- No MCP envelope types in the domain model.
- No UI view-model concerns in shared contracts.

## Compatibility rules specific to this subbundle
- Keep the application surface plain so a future host-repo MCP driver can wrap it into `McpToolEnvelope<T>` without changing the underlying contracts.
- Use naming and error categories that will map cleanly into host-repo structured tool responses later.

## Expected deliverables
- Core records and enums in `Abstractions` and `Domain`.
- Serialization tests and JSON golden files for the snapshot root.
- A documented versioning policy for schema evolution.
