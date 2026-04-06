# SB-07 — Risk rules and insights

## Objective
Turn normalized facts into explicit, provenance-aware architectural findings, risk scores, and open questions without losing the fact/insight boundary.

## Milestone / priority / actor
- Milestone: `M3`
- Priority: `P0`
- Primary actor: `Quality architect`

## Depends on
- SB-01
- SB-04
- SB-05
- SB-06

## Read first
- overview/02-architecture-blueprint.md
- overview/12-risk-register.md
- adrs/ADR-005-facts-and-insights-separation.md

## Current CanDoItAll reference files to inspect
- .codex/agents/canonical-model-skeptic.toml
- .codex/agents/runtime-validator.toml

## In scope
- Implement a rule engine or equivalent deterministic evaluation path for architectural risks.
- Produce findings such as cycles, god classes, oversized files, ambiguous DI, suspicious layering, and unresolved persistence patterns.
- Support severity, confidence, rationale, and evidence links for each insight.
- Emit open questions when the engine lacks enough evidence to decide confidently.

## Out of scope
- No free-form LLM inference inside the core rule engine.
- No UI-specific formatting of findings yet.

## Compatibility rules specific to this subbundle
- Keep the finding model rich enough that a future MCP driver can explain both facts and rationale in structured tool responses.
- Allow future host-repo-specific rules to be added without contaminating generic rules.

## Expected deliverables
- Risk rule definitions and evaluation pipeline.
- Finding severity/confidence/category model.
- Golden tests for deterministic findings.
