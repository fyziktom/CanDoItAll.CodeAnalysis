# ADR-008 — ship lightweight repo assets for portability

## Status
Accepted

## Decision

The standalone repo may include lightweight `architecture/adrs/`, `codex/README.md`, and optional `.codex/agents` placeholders because the host repo already uses those conventions.

## Rationale

These assets improve future portability and team/Codex onboarding without forcing host-specific runtime coupling.
