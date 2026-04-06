# Validation for SB-14 — Tests hardening, repo-local Codex assets, and final refactor

## Acceptance criteria
- All required tests and validation commands pass.
- No large or messy files remain without a strong justification.
- The final repo shape is clean, modular, and host-compatible.

## Validation commands
- `dotnet build CanDoItAll.CodeAnalsis.slnx -warnaserror`
- `dotnet test CanDoItAll.CodeAnalsis.slnx`
- `dotnet format --verify-no-changes`
- `pwsh ./eng/Validate-FileLengths.ps1`
- `pwsh ./eng/Validate-SolutionStructure.ps1`

## Blocking stop conditions
- Do not close the bundle if any required validation is red.
- Do not close the bundle if long-file or folder-hygiene debt was left unresolved without justification.

## Evidence expected
- Final build/test/format/validation outputs.
- Refactor and review reports.
- A concise list of remaining non-blocking risks, if any.
