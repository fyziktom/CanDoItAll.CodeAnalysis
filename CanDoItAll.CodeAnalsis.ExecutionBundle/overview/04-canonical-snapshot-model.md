# Canonical snapshot model

## Root shape

```json
{
  "schemaVersion": "1.0.0",
  "generatorVersion": "0.1.0",
  "snapshotId": "snap_...",
  "createdUtc": "2026-04-06T00:00:00Z",
  "request": { },
  "facts": { },
  "insights": { },
  "exports": { },
  "diagnostics": [ ]
}
```

## Top-level rules

- `request` records what was asked for.
- `facts` contain directly extracted data.
- `insights` contain rule-derived findings, scores, and open questions.
- `exports` describe generated files/assets and where they live.
- `diagnostics` explain gaps, ambiguity, unsupported patterns, or failures.

## Core sections inside `facts`

Suggested sections:
- solution inventory
- projects
- documents
- namespaces
- symbols / types / members
- dependency graph
- service registrations
- persistence model
- runtime-independent metrics

## Core sections inside `insights`

Suggested sections:
- findings
- risk summaries
- hot spots
- open questions
- module interpretations
- confidence summaries

## Provenance expectations

Every meaningful fact or insight should be traceable back to:
- source file/path
- symbol or project identifier where applicable
- collector/rule identifier
- confidence/support level where certainty is limited

## Determinism rules

- stable sort order everywhere
- stable IDs wherever records may be linked from findings or UI pages
- schema version bump whenever the persisted format changes incompatibly
- golden-file tests for representative snapshots
