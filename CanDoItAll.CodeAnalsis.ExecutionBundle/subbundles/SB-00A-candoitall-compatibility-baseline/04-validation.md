# Validation for SB-00A — Current CanDoItAll compatibility baseline

## Acceptance criteria
- Naming, settings, and future tool surface are frozen early enough that later subbundles can code against them.
- The bundle explicitly explains how future integration will reuse `CanDoItAll.Mcp.Core` without cloning it.
- Codex has a concrete compatibility reading list from the current host repo.

## Validation commands
- `pwsh ./eng/Validate-SolutionStructure.ps1`
- `dotnet build CanDoItAll.CodeAnalsis.slnx -warnaserror`
- `dotnet test tests/CanDoItAll.CodeAnalytics.Tests.Architecture/CanDoItAll.CodeAnalytics.Tests.Architecture.csproj --filter Compatibility`

## Blocking stop conditions
- Do not continue if the naming map is still ambiguous.
- Do not continue if future MCP integration still requires redesigning the application-layer contracts.
- Do not continue if the standalone repo starts accreting host-specific infrastructure.

## Evidence expected
- Compatibility matrix completed.
- Future settings and `.vscode` snippets added under `reference/`.
- Architecture tests or validation rules updated to protect the naming map.
