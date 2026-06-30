# SB04 Semantic Invariants

## Invariant Contract

- Invariant ID: SB04-INV-001

| Field | Value |
| --- | --- |
| Invariant ID | `SB04-INV-001` |
| Source raw note | `IN-005`, `IN-008` |
| Expected behavior | EF support remains static persistence metadata analysis and does not claim runtime EF query tuning. |
| Disallowed shallow implementation | Do not add EF wording in docs without positive analyzer tests and negative runtime-query-claim checks. |
| Failing-first test | Negative assertions in `bundle://proof/SB04/transcripts/ef-unit-tests.txt` reject N+1, `AsNoTracking`, split-query, and compiled-query advice. |
| Passing test | `bundle://proof/SB04/transcripts/ef-unit-tests.txt` and `bundle://proof/SB04/transcripts/persistence-integration-tests.txt`. |
| Changed source files | `repo://tests/CanDoItAll.CodeAnalytics.Tests.Unit/EfCoreFacts.cs`, `repo://reference/ef-analyzer-capabilities.md`. |
| Production assertions | Production projects do not reference EF Core packages or execute runtime EF queries. |
| Red-team negative case | Runtime-query terms are limited to explicit out-of-scope docs and negative tests. |
| Downstream dependency check | SB07/SB08 package and README descriptions use static EF metadata wording only. |

## Preserved Behavior

- Building a fixture snapshot still discovers `ShopDbContext` and emits the existing partial-support diagnostic `EF0003`.
- Persistence facts are still reached through the public `ICodeAnalyticsApplicationService` build/query flow.
- EF analysis remains part of `CanDoItAll.CodeAnalytics.Facts` for this publishing wave, matching ADR 0001.

## Strengthened Behavior

- Fixture analysis proves both `ShopDbContext` and `ReportingDbContext` are discovered.
- Entity metadata includes table and schema details for `Order` and `ReportingSnapshot`.
- Relationship metadata includes the expected one-to-many navigation pair for customer orders.
- The filtered persistence view can return reporting metadata without leaking unrelated entities into the filtered result.

## Negative Guarantees

- The analyzer does not emit N+1, `AsNoTracking`, split-query, or compiled-query advice from static metadata-only persistence facts.
- Production projects do not carry EF Core package references or runtime EF query execution paths.
- Public capability documentation says runtime EF query optimization is out of scope until a future query-shape analyzer exists with dedicated tests.

## Residuals

- File-length validation still fails on Web/test monoliths that are outside the EF analyzer boundary and are scheduled for SB06/final cleanup.
