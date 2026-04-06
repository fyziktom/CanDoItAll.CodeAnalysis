# Prompt for SB-08 — Snapshot assembly, serialization, and caching

You are implementing **SB-08** inside the repository `CanDoItAll.CodeAnalsis`.

## Goal
Assemble full snapshots, persist them deterministically, and add file-based caching/versioning without coupling the core to host-repo-specific runtime infrastructure.

## Read before coding
- overview/04-canonical-snapshot-model.md
- overview/05-analysis-pipeline.md
- overview/17-naming-settings-and-tool-surface-map.md
- subbundles/SB-08-snapshot-assembly-serialization-and-caching/01-scope.md
- subbundles/SB-08-snapshot-assembly-serialization-and-caching/03-checklist.md
- subbundles/SB-08-snapshot-assembly-serialization-and-caching/04-validation.md
- subbundles/SB-08-snapshot-assembly-serialization-and-caching/05-forbidden-patterns.md
- subbundles/SB-08-snapshot-assembly-serialization-and-caching/06-required-evidence.md

## Also inspect these current CanDoItAll repo files
- src/CanDoItAll.Mcp.Core/Observability/LogModels.cs
- src/CanDoItAll.Mcp.Core/Operations/OperationPrimitives.cs

## Required implementation steps
- Define the snapshot assembly pipeline and failure-aggregation rules.
- Persist snapshots and derived export assets under a stable folder scheme.
- Add recent-index support and cache invalidation based on request/scope/hash inputs.
- Write tests for deterministic serialization and cache hit/miss behavior.

## Cross-cutting guardrails
- Respect the naming map: repo/solution `CanDoItAll.CodeAnalsis`, project family `CanDoItAll.CodeAnalytics.*`, future driver `CanDoItAll.Mcp.CodeAnalytics`.
- Treat `.slnx` as canonical unless a concrete tool blocker proves otherwise.
- Do not clone `CanDoItAll.Mcp.Core` or any host-repo-only MCP runtime types into the standalone libraries.
- Keep facts, insights, and diagnostics separate.
- Keep comments in English and rare. Avoid XML docs unless a public-contract reason clearly justifies them.
- Keep files small and cohesive. If you temporarily create long files for speed, you must split them before closure.
- Avoid dumping-ground folders such as `Helpers`, `Misc`, or `Stuff`.

## Subbundle-specific stop rules
- Do not continue if storage format is non-deterministic.
- Do not continue if cache behavior is opaque or impossible to diagnose.

## Before you call this subbundle done
- run the listed validation commands,
- update or add the required evidence artifacts,
- verify compatibility with the future `CanDoItAll.Mcp.CodeAnalytics` seam,
- do a local cleanup pass on file size and folder hygiene.
