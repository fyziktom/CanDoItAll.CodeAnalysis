# CanDoItAll.CodeAnalsis

`CanDoItAll.CodeAnalsis` is a standalone .NET 10 repository for the future `CanDoItAll.Mcp.CodeAnalytics` engine.

The repository root and canonical solution intentionally keep the `CodeAnalsis` typo for compatibility with the requested transfer shape:

- Repository root: `CanDoItAll.CodeAnalsis`
- Canonical solution: `CanDoItAll.CodeAnalsis.slnx`
- Project and namespace family: `CanDoItAll.CodeAnalytics.*`
- Future host driver: `CanDoItAll.Mcp.CodeAnalytics`

## Solution layout

- `src/CanDoItAll.CodeAnalytics.Abstractions`
- `src/CanDoItAll.CodeAnalytics.Domain`
- `src/CanDoItAll.CodeAnalytics.Workspace`
- `src/CanDoItAll.CodeAnalytics.Facts`
- `src/CanDoItAll.CodeAnalytics.Analysis`
- `src/CanDoItAll.CodeAnalytics.Rendering`
- `src/CanDoItAll.CodeAnalytics.Storage`
- `src/CanDoItAll.CodeAnalytics.Application`
- `src/CanDoItAll.CodeAnalytics.Web`
- `tests/CanDoItAll.CodeAnalytics.Tests.Support`
- `tests/CanDoItAll.CodeAnalytics.Tests.Unit`
- `tests/CanDoItAll.CodeAnalytics.Tests.Integration`
- `tests/CanDoItAll.CodeAnalytics.Tests.Web`
- `tests/CanDoItAll.CodeAnalytics.Tests.Architecture`

## Guardrails

- `.NET 10` is pinned through `global.json`.
- `CanDoItAll.CodeAnalsis.slnx` is the canonical solution file.
- Root build defaults live in `Directory.Build.props`.
- File-length and repository-structure checks live under `eng/`.
- The standalone libraries must stay transport-agnostic and must not copy `CanDoItAll.Mcp.Core`.

## Validation

```powershell
dotnet restore
dotnet build .\CanDoItAll.CodeAnalsis.slnx -warnaserror
dotnet test .\CanDoItAll.CodeAnalsis.slnx --no-build
pwsh .\eng\Validate-FileLengths.ps1
pwsh .\eng\Validate-SolutionStructure.ps1
```
