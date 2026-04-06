# Validation for SB-00 — Repository bootstrap and guardrails

## Acceptance criteria
- A clean checkout restores, builds, and opens using the canonical solution file.
- The intended project graph exists and no obvious dumping-ground folders were introduced.
- Repository guardrails are executable, not merely documented.
- The repo shape already looks portable into the main CanDoItAll source tree.

## Validation commands
- `dotnet restore`
- `dotnet build CanDoItAll.CodeAnalsis.slnx -warnaserror`
- `dotnet test CanDoItAll.CodeAnalsis.slnx --no-build`
- `pwsh ./eng/Validate-FileLengths.ps1`
- `pwsh ./eng/Validate-SolutionStructure.ps1`

## Blocking stop conditions
- Do not proceed if the solution graph still violates the intended layer boundaries.
- Do not proceed if the repo depends on a local copy of host-repo-only MCP infrastructure.
- Do not proceed if file-length or structure validation is only manual.

## Evidence expected
- Solution tree screenshot or textual tree in the execution report.
- Successful output from structure/file-length validation scripts.
- List of created projects and allowed references.
