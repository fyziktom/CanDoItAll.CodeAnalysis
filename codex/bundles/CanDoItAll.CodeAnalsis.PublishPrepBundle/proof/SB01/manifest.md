# SB01 Proof Manifest

## Validator Contract

- Failing-first transcript: `bundle://proof/SB01/transcripts/file-lengths.txt`.
- Passing transcript: `bundle://proof/SB01/transcripts/build.txt`.
- Anti-stub audit transcript: `bundle://proof/SB01/transcripts/anti-stub-audit.txt`.

## Scope

SB01 stabilized the validation baseline before architecture and publishing refactors.

## Source Changes

| File | Purpose | SHA-256 |
| --- | --- | --- |
| `repo://README.md` | Release validation command now uses the proven solution-level test watchdog and Windows-compatible guardrail commands. | `CE3E61E0DFCD602CFB55F6E0280F132AFA99A4894B80B5B642E58254A461F5A3` |
| `repo://codex/README.md` | Points to the moved active bundle and validation matrix. | `29F1C8011A1023E3B29CFC7AA32A22A3C3D176FA46CC81B18471CA4A1B2E44D5` |
| `repo://codex/validation-matrix.md` | Documents release and segmented validation gates, including slow Roslyn/Web test expectations. | `6F909219E1987E2BFD9F2B337378675C48C3F63BFED81EDD342751D91ED4E433` |
| `repo://eng/Validate-FileLengths.ps1` | Replaced unavailable `System.IO.Path.GetRelativePath` usage and reports all hard-limit violations. | `7B6DD79C857018441F921F756812ED2708AAD548C9B22D5A5FADCAD67D7A91C2` |
| `repo://eng/Validate-SolutionStructure.ps1` | Replaced unavailable `System.IO.Path.GetRelativePath` usage in forbidden-folder diagnostics. | `AD9F552894E2EDED2D08F74B1821A2CBB9412887DD269563179C6487443023BF` |

## Transcript Index

| Transcript | Command result | Notes |
| --- | --- | --- |
| `bundle://proof/SB01/transcripts/build.txt` | `ExitCode: 0` | `dotnet build .\CanDoItAll.CodeAnalsis.slnx -warnaserror` passed. |
| `bundle://proof/SB01/transcripts/tests.txt` | `ExitCode: 0` | Full solution tests passed with `--blame-hang-timeout 600s`; Architecture 8, Web 9, Integration 10, Unit 43. |
| `bundle://proof/SB01/transcripts/solution-structure.txt` | `ExitCode: 0` | Structure guardrail passes in Windows PowerShell. |
| `bundle://proof/SB01/transcripts/file-lengths.txt` | `ExitCode: 1` | File-length guardrail now runs and reports real oversized files for SB03/SB06/test cleanup. This is a known release-prep finding, not a shell failure. |
| `bundle://proof/SB01/transcripts/prepared-validator.txt` | `ExitCode: 0` | Prepared-stage bundle validator passed after the bundle move. |
| `bundle://proof/SB01/transcripts/anti-stub-audit.txt` | `ExitCode: 1` | `rg` found no placeholder markers (`NotImplementedException`, `TODO`, `HACK`, `FIXME`, `STUB`, unsupported placeholders). |

## Source Assertions

- `repo://README.md` and `repo://codex/validation-matrix.md` both document the same full-suite command recorded in `tests.txt`.
- `repo://codex/validation-matrix.md` records segmented test commands for diagnosing slow Unit, Integration, and Web projects without weakening the full release gate.
- `repo://eng/Validate-FileLengths.ps1` and `repo://eng/Validate-SolutionStructure.ps1` no longer depend on PowerShell 7-only `System.IO.Path.GetRelativePath`.
- `repo://eng/Validate-FileLengths.ps1` preserves the `350` review threshold and `450` hard limit.

## Progression Decision

Downstream subbundles may proceed. The test hang is resolved as a documented duration/watchdog issue, and guardrail scripts are runnable in the documented shell. The nonzero file-length result remains an intentional work item for later refactor subbundles.
