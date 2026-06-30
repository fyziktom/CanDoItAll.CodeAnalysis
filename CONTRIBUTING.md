# Contributing

Thanks for helping prepare `CanDoItAll.CodeAnalsis` as a reusable CodeAnalytics engine and desktop sandbox.

## Development Setup

1. Install the .NET SDK pinned by `global.json`.
2. Run commands from the repository root in Windows PowerShell or PowerShell 7.
3. Use the canonical solution file, `CanDoItAll.CodeAnalsis.slnx`.

## Validation

Run the segmented release gate before sending changes:

```powershell
dotnet restore
dotnet build .\CanDoItAll.CodeAnalsis.slnx -warnaserror
dotnet test .\tests\CanDoItAll.CodeAnalytics.Tests.Architecture\CanDoItAll.CodeAnalytics.Tests.Architecture.csproj --no-build --blame-hang --blame-hang-timeout 60s --logger "console;verbosity=normal"
dotnet test .\tests\CanDoItAll.CodeAnalytics.Tests.Unit\CanDoItAll.CodeAnalytics.Tests.Unit.csproj --no-build --blame-hang --blame-hang-timeout 600s --logger "console;verbosity=normal"
dotnet test .\tests\CanDoItAll.CodeAnalytics.Tests.Integration\CanDoItAll.CodeAnalytics.Tests.Integration.csproj --no-build --blame-hang --blame-hang-timeout 600s --logger "console;verbosity=normal"
dotnet test .\tests\CanDoItAll.CodeAnalytics.Tests.Web\CanDoItAll.CodeAnalytics.Tests.Web.csproj --no-build --blame-hang --blame-hang-timeout 600s --logger "console;verbosity=normal"
.\eng\Validate-FileLengths.ps1
.\eng\Validate-SolutionStructure.ps1
```

Use `codex/validation-matrix.md` when you need the rationale for the segmented test matrix.

## Architecture Rules

- Keep reusable libraries transport-agnostic.
- Do not copy MCP host runtime contracts into this repository.
- Keep the Web project as a desktop-large sandbox, not a reusable engine package.
- Add new projects only when they clarify ownership, optional dependencies, or a real second driver/addon boundary.
- Document performance changes with evidence, especially for Roslyn, regex, source file reads, and snapshot storage paths.

## Pull Requests

Keep pull requests focused, include tests for behavior changes, update docs when user-visible or package-visible behavior changes, and avoid committing generated output from `bin/`, `obj/`, `.artifacts/`, `output/`, `TestResults/`, screenshots, or local snapshot caches.
