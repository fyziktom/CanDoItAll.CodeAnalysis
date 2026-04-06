# ADR-001 — repo and naming split

## Status
Accepted

## Decision

Use:
- repo root: `CanDoItAll.CodeAnalsis`
- canonical solution file: `CanDoItAll.CodeAnalsis.slnx`
- project/namespace/assembly family: `CanDoItAll.CodeAnalytics.*`
- future host-repo driver: `CanDoItAll.Mcp.CodeAnalytics`

## Rationale

The user explicitly wants the root identity to preserve the typo for transfer convenience.
That typo should not contaminate reusable assemblies and namespaces.

## Consequence

Architecture tests must protect the split and future tool/settings docs must use the correct driver naming.
