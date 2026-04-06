# ADR-002 — Roslyn-first analysis

## Status
Accepted

## Decision

The primary analysis path uses MSBuild + Roslyn to load and inspect solutions.
Optional enrichers may use compiled metadata later, but reflection/runtime loading is never the baseline.

## Rationale

Roslyn sees incomplete and design-time code more safely than runtime loading and keeps the engine usable on partially broken solutions.
