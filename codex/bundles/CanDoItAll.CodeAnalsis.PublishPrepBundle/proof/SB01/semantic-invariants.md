# SB01 Semantic Invariants

## Invariant Contract

- Invariant ID: SB01-INV-001

| Field | Value |
| --- | --- |
| Invariant ID | `SB01-INV-001` |
| Source raw note | `IN-002`, `IN-003`, `IN-004` |
| Expected behavior | Validation baseline is runnable in the documented shell, tests have a reliable watchdog strategy, and file-length findings are real guardrail output. |
| Disallowed shallow implementation | Do not mark validation solved merely because Architecture tests pass or because a script starts before failing on shell/API incompatibility. |
| Failing-first test | `bundle://proof/SB01/transcripts/file-lengths.txt` preserves real hard-limit failures after shell compatibility is fixed. |
| Passing test | `bundle://proof/SB01/transcripts/build.txt`, `bundle://proof/SB01/transcripts/tests.txt`, and `bundle://proof/SB01/transcripts/solution-structure.txt`. |
| Changed source files | `repo://eng/Validate-FileLengths.ps1`, `repo://eng/Validate-SolutionStructure.ps1`, `repo://codex/validation-matrix.md`, `repo://README.md`. |
| Production assertions | Guardrails keep thresholds and solution requirements intact instead of weakening validation. |
| Red-team negative case | A too-short full-suite watchdog can misclassify slow Roslyn/Web tests as a hang; segmented tests document the reliable path. |
| Downstream dependency check | SB02-SB08 depend on the validation matrix and guardrails established here. |

## Shallow-Pass Trap

- A short observation window can make the full suite look hung after Architecture finishes. SB01 rejects that shallow pass by requiring a full solution test command with `--blame-hang-timeout 600s` and by documenting segmented commands for diagnosis.
- A file-length script that merely starts is insufficient. It must run under the documented shell and report every hard-limit violation, not stop at the first one.

## Adversarial Negative Proof

- `bundle://proof/SB01/transcripts/file-lengths.txt` proves the file-length guardrail still fails on real oversized files. This confirms the guardrail was not weakened to make SB01 look green.
- A previous 180-second full-suite hang watchdog aborted Integration during concurrent Roslyn work. The final matrix widens only the solution-level inactivity timeout while keeping shorter segmented watchdogs for suspicious individual projects.

## Semantic Positive Proof

- `bundle://proof/SB01/transcripts/tests.txt` proves all four test projects pass in one solution-level command: Architecture 8, Web 9, Integration 10, Unit 43.
- `bundle://proof/SB01/transcripts/solution-structure.txt` proves repository-structure validation passes after the Windows PowerShell compatibility fix.
- `bundle://proof/SB01/transcripts/build.txt` proves build still passes with warnings as errors after guardrail and documentation changes.

## Anti-Stub Audit

- `bundle://proof/SB01/transcripts/anti-stub-audit.txt` found no placeholder markers in `src`, `eng`, `tests`, or the validation matrix.

## Raw-Note Literal Closure

- `IN-001` and `REQ-001`: bundle moved into `codex/bundles` and still passes the prepared-stage validator.
- `IN-002` and `REQ-002`: build/test/structure validation is reliable; the full-suite hang is documented as duration/watchdog behavior.
- `IN-003` and `REQ-003`: file-length guardrail is executable and reports the release-prep hotspot list for later refactors.
- `IN-004` and `REQ-011`: the XLSX checklist remains in `bundle://outputs/publishing-prep-checklist.xlsx`.

## Residual Risk

File-length violations remain open by design after SB01. They must be reduced or explicitly resolved by later subbundles before final publishing closure.
