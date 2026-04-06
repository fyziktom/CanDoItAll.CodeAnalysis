# ADR-003 — canonical snapshot model first

## Status
Accepted

## Decision

The canonical snapshot is the primary persisted output.
Markdown, Mermaid, and UI views render from it.

## Rationale

This keeps the engine reusable for UI, testing, future MCP drivers, and offline storage without binding it to one presentation format.
