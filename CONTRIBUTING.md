# Contributing

This repository accepts code contributions only from partners who have been explicitly
approved by the maintainer. Unsolicited pull requests are not accepted.

To discuss becoming an approved partner, contact the maintainer on LinkedIn using the
handle `fyziktom`. Wait for approval before preparing or opening a pull request.

## Development Setup

1. Install the .NET SDK pinned by `global.json`.
2. Run commands from the repository root in Windows PowerShell or PowerShell 7.
3. Use the canonical solution file, `CanDoItAll.CodeAnalsis.slnx`.

## Validation

Run the segmented release gate before sending changes:

```powershell
dotnet restore .\CanDoItAll.CodeAnalsis.slnx --configfile .\NuGet.config
dotnet build .\CanDoItAll.CodeAnalsis.slnx --configuration Release --no-restore -warnaserror
.\tools\deployment\nugets\Build-NuGets.ps1 -Configuration Release -NoRestore
```

Use `codex/validation-matrix.md` when you need the rationale for the segmented test matrix.

## Architecture Rules

- Keep reusable libraries transport-agnostic.
- Do not copy MCP host runtime contracts into this repository.
- Keep the Web project as a desktop-large sandbox, not a reusable engine package.
- Add new projects only when they clarify ownership, optional dependencies, or a real second driver/addon boundary.
- Document performance changes with evidence, especially for Roslyn, regex, source file reads, and snapshot storage paths.

## Pull Requests

- Open a pull request only after partner approval.
- Keep changes focused and include tests for behavior changes.
- Update documentation for user-visible or package-visible behavior changes.
- Include exact validation commands and results.
- Do not commit generated output from `bin/`, `obj/`, `artifacts/`, `output/`,
  `TestResults/`, screenshots, or local snapshot caches.
