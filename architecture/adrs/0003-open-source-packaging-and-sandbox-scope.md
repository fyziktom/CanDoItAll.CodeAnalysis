# ADR 0003: Open Source Packaging And Sandbox Scope

## Status

Accepted

## Context

The repository is being prepared for open-source publication. The reusable value is the transport-agnostic CodeAnalytics engine. The Web project is useful for local inspection but is a desktop-large sandbox, not a package or public hosting contract. The future MCP driver is planned separately and must not pull host runtime contracts into these libraries.

## Decision

Publish eight reusable library packages:

- `CanDoItAll.CodeAnalytics.Domain`
- `CanDoItAll.CodeAnalytics.Abstractions`
- `CanDoItAll.CodeAnalytics.Workspace`
- `CanDoItAll.CodeAnalytics.Facts`
- `CanDoItAll.CodeAnalytics.Analysis`
- `CanDoItAll.CodeAnalytics.Rendering`
- `CanDoItAll.CodeAnalytics.Storage`
- `CanDoItAll.CodeAnalytics.Application`

Keep these non-shipping for this wave:

- `CanDoItAll.CodeAnalytics.Web`
- `tools/*`
- `tests/*`
- `tests/fixtures/*`
- `CanDoItAll.Mcp.CodeAnalytics` future driver references

Use MIT repository and package license metadata. Use `eng/Pack-ReleaseProjects.ps1` as the warning-free release package command for packable projects.

## Consequences

- The root README is packaged into each library package, so README claims must stay accurate for all packages.
- Desktop sandbox docs must state desktop-large scope and avoid mobile/medium polish claims.
- Tools and fixtures can remain useful locally without being release artifacts.
- A future host driver can depend on `Application` and wrap `ICodeAnalyticsApplicationService` without copying MCP host runtime contracts into this repo.

## Proof

- `reference/publishing-readiness.md`
- `reference/desktop-sandbox.md`
- `reference/public-api.md`
- `codex/bundles/CanDoItAll.CodeAnalsis.PublishPrepBundle/proof/SB06`
- `codex/bundles/CanDoItAll.CodeAnalsis.PublishPrepBundle/proof/SB07`
