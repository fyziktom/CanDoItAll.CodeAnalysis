# SB-08 — Snapshot assembly, serialization, and caching

## Objective
Assemble full snapshots, persist them deterministically, and add file-based caching/versioning without coupling the core to host-repo-specific runtime infrastructure.

## Milestone / priority / actor
- Milestone: `M3`
- Priority: `P0`
- Primary actor: `Storage maintainer`

## Depends on
- SB-01
- SB-02
- SB-03
- SB-04
- SB-05
- SB-06
- SB-07

## Read first
- overview/04-canonical-snapshot-model.md
- overview/05-analysis-pipeline.md
- overview/17-naming-settings-and-tool-surface-map.md

## Current CanDoItAll reference files to inspect
- src/CanDoItAll.Mcp.Core/Observability/LogModels.cs
- src/CanDoItAll.Mcp.Core/Operations/OperationPrimitives.cs

## In scope
- Compose partial fact collectors and findings into a complete snapshot.
- Serialize snapshots deterministically to JSON and file-system storage.
- Add cache keys, hash/version metadata, and recent-snapshot indexing.
- Support export packaging metadata without tying storage to UI or MCP concerns.

## Out of scope
- No host-repo operation registry or log buffer clone.
- No cloud/object-storage dependency for v1.

## Compatibility rules specific to this subbundle
- The standalone repo should expose plain storage/application services; a future host-repo MCP driver can add `McpToolEnvelope` and async-operation wrapping later.
- Snapshot IDs, timestamps, and summary metadata should still be rich enough for later operation status and recent-history tools.

## Expected deliverables
- Snapshot assembler.
- File-based snapshot repository and cache metadata model.
- Recent snapshot index and deterministic export layout.
