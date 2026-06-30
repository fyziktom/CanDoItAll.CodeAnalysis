# Scope Inventory

## Source Hotspots

| ID | Path | Finding | Owning subbundle |
| --- | --- | --- | --- |
| `HOT-001` | `repo://src/CanDoItAll.CodeAnalytics.Application/Services/CodeAnalyticsApplicationService.Context.Strategy.cs` | 640 lines; focused-context policy, scoring, clustering, strategy records, and selection mixed in one partial file. | `SB03` |
| `HOT-002` | `repo://src/CanDoItAll.CodeAnalytics.Application/Services/CodeAnalyticsApplicationService.Symbols.cs` | 586 lines; symbol search, definitions, members, implementations, references, scoring, and response shaping mixed. | `SB03` |
| `HOT-003` | `repo://src/CanDoItAll.CodeAnalytics.Application/Services/CodeAnalyticsApplicationService.Context.SeedResolution.cs` | 368 lines; seed resolution and scoring policy should be isolated from service facade. | `SB03` |
| `HOT-004` | `repo://src/CanDoItAll.CodeAnalytics.Web/Components/Pages/ContextLab.razor` | 619 lines; desktop sandbox page mixes form, orchestration, rendering, and helpers. | `SB06` |
| `HOT-005` | `repo://src/CanDoItAll.CodeAnalytics.Web/Components/Pages/Snapshots/Context.razor` | 414 lines; snapshot context view should be split into display components. | `SB06` |
| `HOT-006` | `repo://src/CanDoItAll.CodeAnalytics.Web/Components/Pages/Snapshots/Symbols.razor` | 358 lines; symbol search and detail panes should be split. | `SB06` |
| `HOT-007` | `repo://src/CanDoItAll.CodeAnalytics.Facts/Persistence/PersistenceFactCollector.cs` | Dense EF analyzer orchestration over compilation, symbols, entities, relationships, diagnostics. | `SB04` |
| `HOT-008` | `repo://tools/ScenarioEvaluationHarness/Program.cs` | 801-line tool monolith. | `SB07` |
| `HOT-009` | `repo://tools/ComparisonHarness/Program.cs` | 626-line tool monolith. | `SB07` |
| `HOT-010` | `repo://tests/CanDoItAll.CodeAnalytics.Tests.Unit/ApplicationFacts.cs` | 674-line test monolith. | `SB01`, `SB03` |

## Documentation And Publishing Gaps

| ID | Gap | Owning subbundle |
| --- | --- | --- |
| `DOC-001` | Root README is bootstrap-level and does not yet describe install, public API, packages, sandbox, validation, or release flow. | `SB08` |
| `DOC-002` | ADR folder is placeholder-only. | `SB02`, `SB08` |
| `DOC-003` | Missing license, security policy, contributing guide, code of conduct decision, changelog/release notes. | `SB07`, `SB08` |
| `DOC-004` | Tool-surface and settings reference must be updated only after final boundaries are decided. | `SB07`, `SB08` |
| `DOC-005` | README validation commands mention `pwsh`, but `pwsh` is not available in preparation environment. | `SB01`, `SB08` |

## Performance And EF Scan Items

| ID | Signal | Path | Owning subbundle |
| --- | --- | --- | --- |
| `PERF-001` | 453 LINQ hit lines and 68 dictionary/list allocation hit lines in production scan. | `repo://src` | `SB03`, `SB05` |
| `PERF-002` | Per-query `new Regex(... RegexOptions.Compiled ...)`. | `repo://src/CanDoItAll.CodeAnalytics.Application/Services/CodeAnalyticsApplicationService.Symbols.Source.cs` | `SB05` |
| `PERF-003` | Source and document paths read entire files. | `repo://src/CanDoItAll.CodeAnalytics.Application/Services` | `SB05` |
| `EF-001` | Production app does not execute EF runtime queries; EF work is static analyzer hardening. | `repo://src/CanDoItAll.CodeAnalytics.Facts/Persistence` | `SB04` |
| `EF-002` | Fixture has minimal EF runtime query coverage and no N+1/read-only query examples. | `repo://tests/fixtures/Fixture.Shop` | `SB04` |

## XLSX Artifact

- Planned final workbook: `bundle://outputs/publishing-prep-checklist.xlsx`.
- The workbook must include sheets for summary, subbundle plan, checklist, hotspots, extraction candidates, performance/EF scan, and documentation plan.
