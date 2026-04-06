# SB-10 — Application orchestrator and query API

## Objective
Expose stable application-layer operations that build snapshots, fetch summaries, enumerate exports, and answer focused questions without coupling callers to Roslyn internals.

## Milestone / priority / actor
- Milestone: `M4`
- Priority: `P0`
- Primary actor: `Application maintainer`

## Depends on
- SB-08
- SB-09

## Read first
- overview/01-executive-summary.md
- overview/11-future-mcp-handoff.md
- overview/17-naming-settings-and-tool-surface-map.md

## Current CanDoItAll reference files to inspect
- src/CanDoItAll.Mcp.ProjectStructure/ProjectStructureCoordinator.cs
- src/CanDoItAll.Mcp.Components/Tools/ComponentsTools.cs
- src/CanDoItAll.Mcp.Core/Contracts/McpToolEnvelope.cs

## In scope
- Define application services/handlers for build snapshot, get summary, list recent snapshots, get diagram/export, and query snapshot data.
- Support progress reporting and cancellation in a transport-agnostic way.
- Normalize error categories and results so a future MCP driver can wrap them cleanly.

## Out of scope
- No actual MCP transport project yet.
- No UI-specific service return types in the application layer.

## Compatibility rules specific to this subbundle
- Service boundaries should map almost one-to-one to future `code_analytics_*` MCP tools.
- Plain result contracts must be mappable into `McpToolEnvelope<T>` later without structural changes.

## Expected deliverables
- Application service interfaces and default implementation.
- Progress/result contracts and query API.
- Tests proving orchestration order and error mapping.
