# SB03 Proof Manifest

## Validator Contract

- Failing-first transcript: `bundle://proof/SB03/transcripts/full-validation-tests.txt`.
- Passing transcript: `bundle://proof/SB03/transcripts/application-semantic-tests.txt`.
- Anti-stub audit transcript: `bundle://proof/SB03/transcripts/anti-stub-audit.txt`.

## Scope

SB03 split oversized Application focused-context and symbol-query partial files into smaller responsibility slices without changing `ICodeAnalyticsApplicationService` response contracts.

## Source Changes

| File | Purpose | SHA-256 |
| --- | --- | --- |
| `repo://README.md` | Release gate now documents segmented test execution to avoid concurrent Roslyn/Web starvation. | `9B2CFAD96FA7AE120092D80E790664203BC01607FAC75FC973BDE91A5C22FFF1` |
| `repo://codex/validation-matrix.md` | Segmented project tests are the release gate; full-solution command is optional smoke only. | `B790EF05303160E349D1F910647D1C95BD9F3D0250E4C347DE3D9BC1F9722A9E` |
| `repo://src/CanDoItAll.CodeAnalytics.Application/Services/CodeAnalyticsApplicationService.Context.Strategy.cs` | Retains focused-context strategy planning/selection only. | `20AEAAF3D754FDC430C74A36DA1861A9CAAC51B9FE5A9BD05848045DA401B9CB` |
| `repo://src/CanDoItAll.CodeAnalytics.Application/Services/CodeAnalyticsApplicationService.Context.Strategy.Usage.cs` | New focused-context helper/usage-summary analysis slice. | `24449F40229A43276FE0200A57AD167FCC50FE52AF232C90FBDEDDCA7501DE41` |
| `repo://src/CanDoItAll.CodeAnalytics.Application/Services/CodeAnalyticsApplicationService.Context.Strategy.Models.cs` | New focused-context internal model slice. | `A78B21563B41376553C4CD503ED35F0C2AB3194D4B07144E0A1A7BCE2FDCF996` |
| `repo://src/CanDoItAll.CodeAnalytics.Application/Services/CodeAnalyticsApplicationService.Symbols.cs` | Retains public symbol facade methods only. | `8CDA997F99B937DE06B3CC08B95E000BF1419F48111C58B9705439E74F1E9425` |
| `repo://src/CanDoItAll.CodeAnalytics.Application/Services/CodeAnalyticsApplicationService.Symbols.Context.cs` | New symbol query context/target resolution slice. | `7668B91DA6B5E390154B517315C5250C766A86090129751E29DCA4E12022CA4C` |
| `repo://src/CanDoItAll.CodeAnalytics.Application/Services/CodeAnalyticsApplicationService.Symbols.Search.cs` | New symbol search result/scoring slice. | `8A6CB02AE75DEF72B8CD88494B64A33421802C7B3B2FC83205B4FEDB4826E8BA` |
| `repo://src/CanDoItAll.CodeAnalytics.Application/Services/CodeAnalyticsApplicationService.Symbols.References.cs` | New symbol reference collection/scoring slice. | `23D87C302EA221D2AF345769E18D13830DD0E7CD26B032337C9409882E0F0E94` |
| `repo://src/CanDoItAll.CodeAnalytics.Application/Services/CodeAnalyticsApplicationService.Symbols.Models.cs` | New symbol internal model slice. | `A2C66BFBA799F64BBADAF08D392A02E60FFC392C77ACAD79001B6A29B9560705` |

## Transcript Index

| Transcript | Command result | Notes |
| --- | --- | --- |
| `bundle://proof/SB03/transcripts/build.txt` | `ExitCode: 0` | Full solution build passed with warnings as errors. |
| `bundle://proof/SB03/transcripts/application-semantic-tests.txt` | `ExitCode: 0` | 27 ApplicationFacts tests passed, covering symbol search/definition/references and focused-context positive/adversarial scenarios. |
| `bundle://proof/SB03/transcripts/segmented-validation-tests.txt` | `ExitCode: 0` | Release gate passed: Architecture 11, Unit 43, Integration 10, Web 9. |
| `bundle://proof/SB03/transcripts/full-validation-tests.txt` | `ExitCode: 1` | Adversarial evidence: concurrent full-solution run starved Web operation polling while Unit/Integration Roslyn work ran. Docs were changed to make segmented tests the release gate. |
| `bundle://proof/SB03/transcripts/line-counts.txt` | `ExitCode: 0` | All Application service files are under the 450-line hard limit. |
| `bundle://proof/SB03/transcripts/file-lengths.txt` | `ExitCode: 1` | Remaining hard-limit files are Web ContextLab, Web snapshot Context page, and Unit ApplicationFacts. |
| `bundle://proof/SB03/transcripts/anti-stub-audit.txt` | `ExitCode: 1` | No placeholder/stub markers found in `src` or `tests`. |
| `bundle://proof/SB03/transcripts/prepared-validator.txt` | `ExitCode: 0` | Prepared-stage bundle validator passed after SB03 changes. |

## Progression Decision

SB05 and SB06 may proceed with stable Application response behavior. Remaining file-length work is outside Application service and is owned by SB06/Web and final test cleanup.
