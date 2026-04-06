# Prompt for SB-07 — Risk rules and insights

You are implementing **SB-07** inside the repository `CanDoItAll.CodeAnalsis`.

## Goal
Turn normalized facts into explicit, provenance-aware architectural findings, risk scores, and open questions without losing the fact/insight boundary.

## Read before coding
- overview/02-architecture-blueprint.md
- overview/12-risk-register.md
- adrs/ADR-005-facts-and-insights-separation.md
- subbundles/SB-07-risk-rules-and-insights/01-scope.md
- subbundles/SB-07-risk-rules-and-insights/03-checklist.md
- subbundles/SB-07-risk-rules-and-insights/04-validation.md
- subbundles/SB-07-risk-rules-and-insights/05-forbidden-patterns.md
- subbundles/SB-07-risk-rules-and-insights/06-required-evidence.md

## Also inspect these current CanDoItAll repo files
- .codex/agents/canonical-model-skeptic.toml
- .codex/agents/runtime-validator.toml

## Required implementation steps
- Define rule metadata including stable identifiers, category, severity defaults, and rationale templates.
- Evaluate rules against the canonical fact graph rather than raw Roslyn objects.
- Record evidence links for every produced finding.
- Support “insufficient evidence” or “open question” results where certainty is inappropriate.

## Cross-cutting guardrails
- Respect the naming map: repo/solution `CanDoItAll.CodeAnalsis`, project family `CanDoItAll.CodeAnalytics.*`, future driver `CanDoItAll.Mcp.CodeAnalytics`.
- Treat `.slnx` as canonical unless a concrete tool blocker proves otherwise.
- Do not clone `CanDoItAll.Mcp.Core` or any host-repo-only MCP runtime types into the standalone libraries.
- Keep facts, insights, and diagnostics separate.
- Keep comments in English and rare. Avoid XML docs unless a public-contract reason clearly justifies them.
- Keep files small and cohesive. If you temporarily create long files for speed, you must split them before closure.
- Avoid dumping-ground folders such as `Helpers`, `Misc`, or `Stuff`.

## Subbundle-specific stop rules
- Do not continue if findings lack evidence links.
- Do not continue if facts and insights are merged in one collection.

## Before you call this subbundle done
- run the listed validation commands,
- update or add the required evidence artifacts,
- verify compatibility with the future `CanDoItAll.Mcp.CodeAnalytics` seam,
- do a local cleanup pass on file size and folder hygiene.
