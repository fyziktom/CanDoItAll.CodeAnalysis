# Repository Agent Instructions

## Shared Standards

Follow the reviewed standards in a resolved `CanDoItAll.SharedInfo` clone. This
repository owns its local implementation and the compatibility exceptions documented in
`docs/repository-standards.md`.

Use `$apply-candoitall-shared-standards` when available. It resolves SharedInfo from an
explicit `CANDOITALL_SHAREDINFO_ROOT` or nearby sibling locations.

## Repository Scope

- This repository owns the reusable CodeAnalytics engine, its NuGet packages, tests,
  local analysis tools, and the non-packable desktop sandbox.
- It does not own the future MCP host driver or the shared CanDoItAll component
  libraries. Shipping dependencies on sibling repositories must use published packages.

## Commands

- Build: `dotnet build .\CanDoItAll.CodeAnalsis.slnx --configuration Release`
- Test: `dotnet test .\CanDoItAll.CodeAnalsis.slnx --configuration Release`
- Validate and pack: `.\tools\deployment\nugets\Build-NuGets.ps1`

## Safety

- Keep sibling repositories read-only unless the user explicitly requests a multi-repo
  change.
- Do not commit generated output, local settings, credentials, or runtime state.
- Preserve repository-specific changes that are unrelated to the active task.
