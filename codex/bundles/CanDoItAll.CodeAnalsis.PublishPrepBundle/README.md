# Preparation for publishing CanDoItAll.CodeAnalytics as open source

This initiative bundle prepares `CanDoItAll.CodeAnalytics` for an open-source publishing wave. It does not authorize implementation yet; it gives the next agent a dependency-aware execution plan, acceptance gates, proof requirements, and detailed checklist artifacts.

## Profile

- `initiative`

## Mission

- Review, harden, refactor, document, and package the app so it can be published as an open-source code-analysis engine plus desktop sandbox UI without hiding maintainability, performance, EF analyzer, documentation, validation, or publishing risks.

## Outcome Contract

- Requested outcome: implementation-ready release-preparation bundle for open-source publishing.
- Hard constraints: do not implement code during preparation; preserve the existing `CanDoItAll.CodeAnalsis` repository and solution spelling unless a later subbundle explicitly decides otherwise; treat the sandbox UI as desktop-large-screen only; base documentation changes on the improvements actually shipped by earlier subbundles.
- Evidence required before closure: clean build, non-hanging test strategy, file-length/structure guardrails, targeted performance evidence, EF analyzer coverage, large-screen browser proof for UI changes, OSS packaging verification, updated docs, and final raw-input closure.
- Known blockers or explicit scope exceptions: current full-suite `dotnet test --no-build` hung in Unit/Web/Integration test hosts during preparation; `pwsh` is not available on this machine, and `eng/Validate-FileLengths.ps1` fails under Windows PowerShell because `System.IO.Path.GetRelativePath` is unavailable.

## Bundle Layout

- `inputs/` raw request, source-artifact list, and structured input.
- `analysis/` repo-grounded current state, assumptions, risks, performance, and EF scan findings.
- `requirements/` normalized publishing requirements with observable acceptance criteria.
- `architecture/` target boundaries, extraction candidates, and release shape.
- `plan/` execution order, dependency map, critical foundations, and gates.
- `traceability/` requirement-to-subbundle mapping.
- `inventories/` source, documentation, hotspot, and extraction checklists.
- `templates/` reusable subbundle template.
- `shared-prompts/` implementation and QA prompts.
- `subbundles/` numbered execution-ready workstreams.
- `reviews/` self-review and execution report seed.

## Recommended Execution Order

1. `subbundles/01-validation-baseline-and-release-guardrails`
2. `subbundles/02-architecture-seams-and-project-extraction`
3. `subbundles/03-application-service-and-focused-context-refactor`
4. `subbundles/04-facts-collectors-and-ef-analyzer-hardening`
5. `subbundles/05-storage-rendering-export-and-performance-hardening`
6. `subbundles/06-desktop-sandbox-ui-decomposition`
7. `subbundles/07-open-source-packaging-and-publishing-readiness`
8. `subbundles/08-documentation-overhaul-and-final-closure-audit`

## Dependency And Validation Map

- Keep the mermaid dependency map, critical-subbundle notes, and phase gates current in `plan/01-phase-plan.md`.
- If the bundle is resumed after compaction or by a different agent, use this README, the current subbundle README, and `reviews/01-execution-report.md` as the durable state.

## Validation Summary

- Bundle preparation status: `Prepared`
- Bundle readiness gate: `Passed prepared-stage validator`
- Execution status: `Completed`
- Subbundle gate review: `Seeded for execution`
- Final closure gate: `Passed`
- Browser validation analytics: `Passed at 1600x1000 desktop-large viewport`
