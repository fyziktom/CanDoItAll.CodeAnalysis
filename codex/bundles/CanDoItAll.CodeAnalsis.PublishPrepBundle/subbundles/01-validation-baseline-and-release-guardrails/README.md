# Validation Baseline And Release Guardrails

## Status

- `Completed`

## Objective

- Make the release validation baseline trustworthy before any refactoring or publishing work starts.

## Success Criteria

- Build, tests, file-length, solution-structure, and prepared/completed bundle validators have reliable commands and captured transcripts.
- Current full-suite test hang is diagnosed, fixed, or split into a documented reliable validation matrix.
- File-length guardrail runs in the documented shell or the docs/script are made compatible.

## Covered Inputs

- `IN-001`
- `IN-002`
- `IN-003`
- `IN-004`
- `REQ-001`
- `REQ-002`
- `REQ-003`
- `REQ-011`

## Prerequisites

- No implementation subbundle may start before this baseline gate is executed.
- Reopen `bundle://analysis/01-current-state.md` and confirm whether the test hang still reproduces.

## Exact Source References

- `repo://README.md`
- `repo://CanDoItAll.CodeAnalsis.slnx`
- `repo://global.json`
- `repo://eng/Validate-FileLengths.ps1`
- `repo://eng/Validate-SolutionStructure.ps1`
- `repo://tests/CanDoItAll.CodeAnalytics.Tests.Unit/ApplicationFacts.cs`
- `repo://tests/CanDoItAll.CodeAnalytics.Tests.Integration/OrchestratorFacts.cs`
- `repo://tests/CanDoItAll.CodeAnalytics.Tests.Web/WebUiFacts.cs`
- `bundle://analysis/01-current-state.md`

## Deliverables

- Reliable validation command matrix for build, test, file-length, solution structure, package validation, and bundle validators.
- Fix or documented split for the full-suite `dotnet test --no-build` hang.
- Fix or documented shell requirement for `Validate-FileLengths.ps1`.
- Final `.xlsx` checklist artifact at `bundle://outputs/publishing-prep-checklist.xlsx`.
- `proof/SB01/manifest.md` and `proof/SB01/semantic-invariants.md`.

## Dependency Impact

- `SB02` through `SB08` depend on this phase because all later refactors need stable gates. If tests or guardrail scripts remain flaky, downstream proof cannot be trusted.

## Validation Depth

- Critical foundation.
- Requires Semantic Adequacy Gate proof: shallow-pass trap, adversarial negative proof, semantic positive proof, anti-stub audit, raw-note literal closure.

## Implementation Steps

1. Reproduce the preparation-time validation status with transcripts.
2. Identify which Unit/Web/Integration process hangs and whether Roslyn MSBuildWorkspace build hosts are orphaned.
3. Add targeted test timeout, fixture cleanup, MSBuildWorkspace disposal, or segmented command guidance as needed.
4. Make file-length validation runnable either by documenting PowerShell 7 or by replacing incompatible API usage.
5. Record exact validation command matrix in repo docs and execution report.
6. Generate or update the XLSX checklist if it changes during execution.
7. Run build, segmented tests, solution structure, file length, and prepared-stage bundle validator.

## Scope Exceptions

- Do not refactor production architecture beyond what is necessary to make validation reliable.
- Do not change public package metadata; `SB07` owns publishing metadata.

## Do Not Do

- Do not start project extraction.
- Do not ignore the full-suite hang by only running architecture tests.
- Do not weaken the file-length threshold without an explicit architecture decision.

## Acceptance Checklist

- Build transcript shows `dotnet build .\CanDoItAll.CodeAnalsis.slnx -warnaserror` success.
- Test transcripts show either full-suite success or an explicit segmented matrix with no silent hang.
- File-length validation transcript runs successfully in the documented environment.
- Solution-structure validation transcript passes.
- XLSX artifact exists and is visually verified.
- `reviews/01-execution-report.md` records closure gate and downstream progression result.

## Proof Required

- `bundle://proof/SB01/manifest.md`
- `bundle://proof/SB01/semantic-invariants.md`
- `bundle://proof/SB01/transcripts/build.txt`
- `bundle://proof/SB01/transcripts/tests.txt`
- `bundle://proof/SB01/transcripts/file-lengths.txt`
- `bundle://proof/SB01/transcripts/solution-structure.txt`
- `bundle://proof/SB01/transcripts/prepared-validator.txt`
- Source assertions proving validation commands in docs/scripts match the transcripts.
- Anti-stub audit transcript for production and guardrail scripts.

## Browser Validation Logging

- N/A: this subbundle does not change browser-visible UI.

## Progression Gate

- Downstream subbundles may start. Validation commands are now reliable enough that a future build, test, or structure failure means a real regression. File-length validation still fails on known oversized files and remains an explicit refactor target for later subbundles.

## Suggested Agent Prompt

```text
Implement SB01 only. Diagnose and stabilize the validation baseline, capture transcripts, update the execution report, create proof/SB01 artifacts, and stop before architecture or UI refactors.
```
