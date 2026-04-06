# ADR-005 — facts and insights separation

## Status
Accepted

## Decision

Facts, insights, and diagnostics remain top-level distinct concepts in both contracts and storage.

## Rationale

Architecture agents need to tell what is proven by code apart from what is inferred or uncertain.
