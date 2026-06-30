# Assumptions And Risks

## Assumptions

- The public package family remains `CanDoItAll.CodeAnalytics.*`; the repository and solution spelling `CodeAnalsis` is preserved for compatibility unless a future publishing decision explicitly changes it.
- The Web project is a desktop sandbox for large screens, not a responsive consumer-facing web app.
- The future MCP host driver should stay thin and transport-specific; core analysis libraries should remain transport-agnostic.
- Refactors must keep observable snapshot facts, stable IDs, exported artifact paths, and current tool-surface semantics compatible unless a subbundle explicitly updates the contract and tests.
- Documentation should be written after implementation subbundles finalize boundaries, not before.

## Critical Path Risks

- `SB01` is a critical foundation because later work cannot be trusted while full-suite tests hang and file-length guardrails cannot run reliably in the documented shell.
- `SB02` is a critical foundation because project extraction decisions affect every later refactor, package boundary, and documentation artifact.
- `SB03` is a critical foundation because `CodeAnalyticsApplicationService` owns most public application workflows; a weak split can break symbol, context, export, and UI flows.
- `SB04` is a critical foundation because EF/persistence analyzer behavior is a core differentiator and may be extracted into an addon; weak proof can invalidate docs and package claims.
- `SB08` is critical closure because documentation and raw input closure must reflect the actual shipped implementation, not this preparation forecast.

## Validation Risks

- Existing `dotnet test --no-build` hang means implementation must first isolate the blocking project/test and define a reliable full-suite or segmented validation plan.
- `eng/Validate-FileLengths.ps1` cannot be trusted until the `pwsh` dependency or Windows PowerShell compatibility issue is fixed.
- Performance findings are static-scan signals; any micro-optimization must be backed by benchmark, scenario harness, or targeted before/after measurement.
- EF Core query guidance must not invent runtime EF query work in production `src`; proof should focus on analyzer behavior and fixture coverage unless a real EF runtime path is introduced.
- UI proof must not be replaced with prose. Desktop sandbox changes need large-screen Playwright/browser evidence.
- OSS publishing proof may require network, package tooling, or NuGet metadata validation that is environment-sensitive.

## Reopen Triggers

- Reopen `SB01` if any later test, build, file-length, or structure command hangs, fails, or needs an undocumented environment assumption.
- Reopen `SB02` if a later subbundle discovers an extraction boundary that creates circular references, duplicate contracts, or host-specific leakage.
- Reopen `SB03` if focused-context or symbol behavior changes without semantic negative and positive proof.
- Reopen `SB04` if persistence facts, EF diagnostics, model snapshot parsing, or fixture coverage become inconsistent after extraction.
- Reopen `SB05` if performance work changes snapshot IDs, export ordering, file paths, or public response shapes.
- Reopen `SB06` if large-screen browser evidence shows clipping, unreadable dense data, broken workspace picker behavior, or awkward desktop layouts.
- Reopen `SB07` if package validation exposes missing metadata, license ambiguity, or accidental inclusion of Web/tools/test artifacts.
- Reopen `SB08` if documentation claims a project, command, package, driver, or feature that did not actually ship with proof.
