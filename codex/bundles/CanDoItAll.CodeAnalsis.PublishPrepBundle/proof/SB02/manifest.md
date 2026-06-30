# SB02 Proof Manifest

## Validator Contract

- Failing-first N/A process/non-production exemption: SB02 boundary tests are newly added and would fail on forbidden dependency directions.
- Passing transcript: `bundle://proof/SB02/transcripts/architecture-tests.txt`.
- Anti-stub audit transcript: `bundle://proof/SB02/transcripts/anti-stub-audit.txt`.

## Scope

SB02 decided publishing boundaries and protected them with architecture tests before service-level refactors begin.

## Source Changes

| File | Purpose | SHA-256 |
| --- | --- | --- |
| `repo://architecture/adrs/0001-publishing-boundaries.md` | ADR deciding current, internal, future-addon, and future-driver boundaries. | `CE63577212F5BBC62989DC1A09E9BA91EB19EDE4B44AE5067C69FE19E3CD0860` |
| `repo://architecture/adrs/README.md` | ADR index points to the active publishing bundle and new ADR. | `5BC3DF315A17B0AE9AE92CB2F7D6F1EEE659363925A3F9EE16219201957D1C3E` |
| `repo://tests/CanDoItAll.CodeAnalytics.Tests.Architecture/SolutionStructureFacts.cs` | Added acyclic graph, sandbox/non-shipping, and Web SDK boundary tests. | `BC70CB708596B7A9FAE9857FD4B0BCB321E8772D4CD2A1436EACF297B8D8E998` |
| `bundle://architecture/01-target-solution.md` | Candidate table updated with SB02 decisions. | `1A8A793130B6CF58D44F4019E6043186C3F39B5DBF4D13FEE1EE839C4D35099B` |

## Transcript Index

| Transcript | Command result | Notes |
| --- | --- | --- |
| `bundle://proof/SB02/transcripts/build.txt` | `ExitCode: 0` | Full solution build passed with warnings as errors. |
| `bundle://proof/SB02/transcripts/architecture-tests.txt` | `ExitCode: 0` | Architecture suite passed with 11 tests, including the new boundary tests. |
| `bundle://proof/SB02/transcripts/solution-structure.txt` | `ExitCode: 0` | Structure guardrail passed with unchanged solution shape. |
| `bundle://proof/SB02/transcripts/project-graph.txt` | `ExitCode: 0` | Captures current project references, Web SDK use, and solution project entries. |
| `bundle://proof/SB02/transcripts/prepared-validator.txt` | `ExitCode: 0` | Prepared-stage bundle validator passed. |
| `bundle://proof/SB02/transcripts/anti-stub-audit.txt` | `ExitCode: 1` | No placeholder/stub markers found in source, tests, architecture, reference docs, or validation matrix. |

## Boundary Decision Summary

- No new project is created in SB02.
- `Application` remains the engine facade for this publishing wave.
- Focused-context and symbol-query work should be split into internal Application services in SB03.
- EF Core persistence facts remain under `Facts` in SB04 while being designed as the first future optional fact addon.
- Web remains a non-core desktop sandbox and is the only source project allowed to use `Microsoft.NET.Sdk.Web`.
- The future MCP driver remains outside this repo; engine projects must stay transport-agnostic.

## Progression Decision

Downstream service, facts, storage/performance, UI, packaging, and documentation subbundles may proceed against the stable current project graph and ADR 0001.
