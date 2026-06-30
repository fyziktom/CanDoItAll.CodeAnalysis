# SB02 Semantic Invariants

## Invariant Contract

- Invariant ID: SB02-INV-001

| Field | Value |
| --- | --- |
| Invariant ID | `SB02-INV-001` |
| Source raw note | `IN-002`, `IN-003`, `IN-005` |
| Expected behavior | Boundary decisions are documented and enforced by architecture tests without prematurely creating package projects. |
| Disallowed shallow implementation | Do not only write an ADR; executable tests must protect dependency direction and sandbox isolation. |
| Failing-first test | Architecture tests would fail if reusable projects reference Web/tools/tests or introduce source cycles. |
| Passing test | `bundle://proof/SB02/transcripts/architecture-tests.txt`. |
| Changed source files | `repo://architecture/adrs/0001-publishing-boundaries.md`, `repo://tests/CanDoItAll.CodeAnalytics.Tests.Architecture/SolutionStructureFacts.cs`. |
| Production assertions | Reusable source projects stay transport-agnostic and Web remains the only Web SDK source project. |
| Red-team negative case | Creating projects for every candidate would increase package churn without proving maintainability. |
| Downstream dependency check | SB03-SB08 follow ADR 0001 for internal services, package boundaries, and future driver/addon posture. |

## Shallow-Pass Trap

- Merely writing an ADR is not sufficient. SB02 adds executable architecture tests for the core decisions: acyclic source project graph, sandbox isolation, and Web SDK isolation.
- Creating projects for every candidate would look like progress but would increase package churn before consumer pressure exists. ADR 0001 rejects project splits that only move files.

## Adversarial Negative Proof

- `Production_project_reference_graph_is_acyclic` would fail if any source project introduced a reference cycle.
- `Reusable_source_projects_do_not_reference_sandbox_or_nonshipping_boundaries` would fail if reusable engine libraries started depending on Web, tests, or tools.
- `Desktop_sandbox_is_the_only_web_sdk_source_project` would fail if a reusable library became a Web SDK project.

## Semantic Positive Proof

- `bundle://proof/SB02/transcripts/architecture-tests.txt` proves all 11 architecture tests pass after the new constraints were compiled.
- `bundle://proof/SB02/transcripts/project-graph.txt` captures the current `.slnx` entries and `ProjectReference` surface used by those tests.
- `repo://architecture/adrs/0001-publishing-boundaries.md` documents a candidate-by-candidate decision, migration order, and rollback plan.

## Anti-Stub Audit

- `bundle://proof/SB02/transcripts/anti-stub-audit.txt` found no placeholder markers across source, tests, architecture, reference docs, or validation matrix.

## Raw-Note Literal Closure

- `IN-002` and `REQ-009`: messy architecture boundaries are now classified into current projects, internal services, future addons, and future driver work.
- `IN-003`: long-file pressure is routed to internal service/component splits instead of premature package churn.
- `IN-005` and `REQ-004`: project/helper/driver/addon decisions are documented in ADR 0001 and protected by tests.
- `REQ-010`: documentation work remains intentionally limited to architecture decisions; full docs update waits for shipped refactors in SB08.

## Residual Risk

The ADR intentionally defers optional package splits. If SB03 or SB04 reveals a dependency that cannot be kept clean inside the current projects, reopen SB02 before adding a new project.
