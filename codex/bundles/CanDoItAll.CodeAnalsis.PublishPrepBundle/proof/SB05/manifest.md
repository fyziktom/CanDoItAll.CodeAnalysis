# SB05 Proof Manifest

## Scope

SB05 hardened selected storage, export, regex-search, and source-read paths based on the performance scan findings. The changes prioritize bounded public behavior and security over unmeasured broad LINQ rewrites.

## Source Changes

| File | Purpose | SHA-256 |
| --- | --- | --- |
| `repo://src/CanDoItAll.CodeAnalytics.Storage/Snapshots/FileSnapshotRepository.cs` | Adds write-time export path containment under the snapshot directory. | `F055C97D88170F40DB1CFA892E7987C769738C5F191B32259FC015D152447F9F` |
| `repo://src/CanDoItAll.CodeAnalytics.Application/Services/CodeAnalyticsApplicationService.Symbols.Source.cs` | Replaces dynamic compiled regex with timeout-bounded matching and fail-closed timeout handling. | `FD99AF315EC2EF32983C6797C2ACB88DC6838586969C620DF5DCCFF9F748C4A2` |
| `repo://src/CanDoItAll.CodeAnalytics.Application/Services/CodeAnalyticsApplicationService.Context.Excerpts.cs` | Adds shared source path containment and 2 MB source-read limit for excerpts. | `F0A21B9B8E50FA1428CBF706B8AD0ED7E3D98710F1C822F5DFA15CF9F19D27E6` |
| `repo://src/CanDoItAll.CodeAnalytics.Application/Services/CodeAnalyticsApplicationService.Documents.cs` | Routes document source reads through the bounded source resolver. | `676C17C63D2653AFDF307102335129A3D570448A2D54B927A3CB71ED56BF504E` |
| `repo://tests/CanDoItAll.CodeAnalytics.Tests.Unit/SnapshotRepositoryFacts.cs` | Proves export traversal paths are rejected and not written. | `2D3326320C4F26E263FB6A536BC9E239054CB8C98B0970EFF34901178CF21B08` |
| `repo://tests/CanDoItAll.CodeAnalytics.Tests.Unit/ApplicationSafetyFacts.cs` | Proves regex timeout safety, source path containment, and source size limit through public app APIs. | `98810D05723947DC90CAC576025A459EAE400E3DE7729D6DD443922DAA9E9FAA` |
| `repo://reference/performance-hardening-notes.md` | Documents SB05 regex, source-read, export-write, and deferred optimization decisions. | `C715A8305E0D27F209F53FA203E187E3410BB64AE048A3A7CBD9D5C25DA28D87` |

## Transcript Index

| Transcript | Command result | Notes |
| --- | --- | --- |
| `bundle://proof/SB05/transcripts/performance-scan-before.txt` | `ExitCode: 0` | Exact scan checklist before SB05 changes; dynamic regex had `RegexOptions.Compiled`, source reads were unbounded. |
| `bundle://proof/SB05/transcripts/build.txt` | `ExitCode: 0` | Full solution build passed with warnings as errors. |
| `bundle://proof/SB05/transcripts/safety-unit-tests.txt` | `ExitCode: 0` | 5 storage/application safety tests passed. |
| `bundle://proof/SB05/transcripts/rendering-serialization-tests.txt` | `ExitCode: 0` | 6 rendering/serialization tests passed. |
| `bundle://proof/SB05/transcripts/performance-scan-after.txt` | `ExitCode: 0` | Dynamic `RegexOptions.Compiled` count in selected app path dropped to 0; source reads are now behind bounds. |
| `bundle://proof/SB05/transcripts/source-safety-assertions.txt` | `ExitCode: 0` | Confirms regex timeout, source-read limit, and export path containment source points. |
| `bundle://proof/SB05/transcripts/file-lengths.txt` | `ExitCode: 1` | Residual Web/test hard-limit files remain for SB06/final cleanup. |
| `bundle://proof/SB05/transcripts/anti-stub-audit.txt` | `ExitCode: 0` | Hits are UI placeholder attributes and an existing compatibility note, not implementation stubs. |
| `bundle://proof/SB05/transcripts/prepared-validator.txt` | `ExitCode: 0` | Prepared-stage bundle validator passed after SB05 changes. |

## Progression Decision

SB06 and SB07 may proceed. Storage/export writes are contained, source reads are bounded for public responses, and dynamic regex search is timeout guarded. Broad LINQ/rendering rewrites were deferred because no selected evidence proved them as bottlenecks.
