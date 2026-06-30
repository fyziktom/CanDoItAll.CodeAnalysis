# SB03 Semantic Invariants

## Invariant Contract

- Invariant ID: SB03-INV-001

| Field | Value |
| --- | --- |
| Invariant ID | `SB03-INV-001` |
| Source raw note | `IN-003`, `IN-005`, `IN-007` |
| Expected behavior | Application focused-context and symbol responsibilities are split into smaller files while preserving `ICodeAnalyticsApplicationService` behavior. |
| Disallowed shallow implementation | Do not compile-only refactor or return non-empty context without preserving concrete symbol/focused-context semantics. |
| Failing-first test | `bundle://proof/SB03/transcripts/full-validation-tests.txt` captures the concurrent full-suite starvation case that docs must not use as the release gate. |
| Passing test | `bundle://proof/SB03/transcripts/application-semantic-tests.txt` and `bundle://proof/SB03/transcripts/segmented-validation-tests.txt`. |
| Changed source files | `repo://src/CanDoItAll.CodeAnalytics.Application/Services/CodeAnalyticsApplicationService.Context.Strategy*.cs`, `repo://src/CanDoItAll.CodeAnalytics.Application/Services/CodeAnalyticsApplicationService.Symbols*.cs`. |
| Production assertions | Application service production files are below the hard file-length limit after the split. |
| Red-team negative case | Relation hints that do not match must not fall back to broad helper usage. |
| Downstream dependency check | SB05-SB08 rely on stable Application response behavior and segmented tests. |

## Shallow-Pass Trap

- A compile-only refactor would not prove behavior. `application-semantic-tests.txt` exercises symbol search, symbol definitions, symbol references, focused-context seed resolution, high-fan-in helper mode, relation-hint filtering, outline precision, and selection reasons.
- A non-empty focused-context result is insufficient. Existing tests assert concrete members, files, usage-summary counts, omitted callers, relation-hint filtering, and absence of broad fallback consumers.

## Adversarial Negative Proof

- `Application_does_not_fall_back_to_broad_helper_usage_when_relation_hints_do_not_match` proves missing relation hints do not silently return broad helper usage.
- `Application_can_render_usage_summary_without_pulling_consumer_members_into_main_selection` proves usage summaries do not pollute surgical member selection.
- `full-validation-tests.txt` proves concurrent full-solution tests can produce false Web failures under resource pressure; `segmented-validation-tests.txt` is the corrected release gate.

## Semantic Positive Proof

- Symbol workflows still return exact type search results, member definitions with source excerpts, implementation lists, member invocation references, and type dependency references.
- Focused-context workflows still resolve service seeds, prompt/diagnostic text, behavior-intent compatibility, high-fan-in helper surgical mode, and outline precision without code excerpts.
- Application service production line counts are now bounded: the largest Application service file is `CodeAnalyticsApplicationService.Context.SeedResolution.cs` at 429 lines.

## Anti-Stub Audit

- `anti-stub-audit.txt` found no placeholder markers in source or tests.

## Raw-Note Literal Closure

- `IN-003` and `REQ-003`: oversized Application service files were split below the hard limit.
- `IN-005` and `REQ-005`: Application remains the facade, with focused-context and symbol responsibilities split into smaller internal slices per ADR 0001.
- `IN-007` and `REQ-007`: no blind performance rewrite was made; behavior-preserving ownership refactor came first.

## Residual Risk

File-length validation still fails on Web pages and the large Unit `ApplicationFacts.cs` test file. Those are not Application service production files and remain in scope for SB06/final cleanup.
