# SB04 Proof Manifest

## Validator Contract

- Failing-first N/A process/non-production exemption: SB04 strengthens an existing static analyzer boundary with positive and negative assertions; no production runtime-query feature was added for a failing-first implementation transcript.
- Passing transcript: `bundle://proof/SB04/transcripts/ef-unit-tests.txt`.
- Anti-stub audit transcript: `bundle://proof/SB04/transcripts/anti-stub-audit.txt`.

## Scope

SB04 hardened the EF/persistence analyzer boundary by documenting that EF support is static Roslyn fact collection, not runtime EF query execution or query tuning. The implementation added positive and adversarial tests for persistence facts and query-tuning claim suppression.

## Source Changes

| File | Purpose | SHA-256 |
| --- | --- | --- |
| `repo://tests/CanDoItAll.CodeAnalytics.Tests.Unit/EfCoreFacts.cs` | Adds store-object, relationship, filtered persistence view, and negative runtime-query-claim assertions. | `C7B7B442B8772CB54DA798FBEF369489173A5F41A7F0A839FC0FEBD057F815C8` |
| `repo://reference/ef-analyzer-capabilities.md` | Documents static EF analyzer capabilities and explicitly excludes runtime EF query tuning claims. | `9AC5335EE11A9C89673FCC807990590E21572F9645198CF02521AEBDB5BC0D2D` |

## Transcript Index

| Transcript | Command result | Notes |
| --- | --- | --- |
| `bundle://proof/SB04/transcripts/build.txt` | `ExitCode: 0` | Full solution build passed with warnings as errors. |
| `bundle://proof/SB04/transcripts/ef-unit-tests.txt` | `ExitCode: 0` | 3 EF unit tests passed, including positive static metadata assertions and negative runtime-query-claim assertions. |
| `bundle://proof/SB04/transcripts/persistence-integration-tests.txt` | `ExitCode: 0` | Persistence integration test passed through the public application service. |
| `bundle://proof/SB04/transcripts/production-ef-source-scan.txt` | `ExitCode: 0` | Production EF hits are static fact/query response references; no production EF LINQ data-access query execution was found. |
| `bundle://proof/SB04/transcripts/ef-package-scan.txt` | `ExitCode: 0` | EF Core package references are confined to fixture projects; production projects do not reference EF Core packages. |
| `bundle://proof/SB04/transcripts/ef-claim-scan.txt` | `ExitCode: 0` | Runtime-query terms appear only in the explicit out-of-scope reference doc and negative tests. |
| `bundle://proof/SB04/transcripts/file-lengths.txt` | `ExitCode: 1` | Residual guardrail failures remain in Web pages and `ApplicationFacts`; they are owned by SB06/final cleanup. |
| `bundle://proof/SB04/transcripts/anti-stub-audit.txt` | `ExitCode: 0` | Hits are UI placeholder attributes and an existing compatibility note, not implementation stubs. |
| `bundle://proof/SB04/transcripts/prepared-validator.txt` | `ExitCode: 0` | Prepared-stage bundle validator passed after SB04 changes. |

## Progression Decision

SB05, SB07, and SB08 may rely on the EF capability boundary: current support is static persistence metadata analysis inside `CanDoItAll.CodeAnalytics.Facts`; runtime EF query optimization claims are not shipped. Remaining file-length closure belongs to later subbundles.
