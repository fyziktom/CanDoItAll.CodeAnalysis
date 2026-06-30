# Current State

## Repository Shape

- Canonical solution: `repo://CanDoItAll.CodeAnalsis.slnx`.
- SDK: `repo://global.json` pins `10.0.200` with `latestPatch`; local resolved SDK during preparation was `10.0.201`.
- Production projects: Abstractions, Domain, Workspace, Facts, Analysis, Rendering, Storage, Application, Web.
- Test projects: Support, Unit, Integration, Web, Architecture.
- Tools: `tools/ScenarioEvaluationHarness` and `tools/ComparisonHarness`.
- Documentation exists but is bootstrap-level: root README, ADR placeholder, Codex notes, compatibility matrix, host-context reference, tool-surface proposal, and settings examples.

## Current Validation Evidence

| Check | Result | Notes |
| --- | --- | --- |
| `git status --short` | Clean | No pre-existing user changes were observed before bundle creation. |
| `dotnet build .\CanDoItAll.CodeAnalsis.slnx -warnaserror` | Passed | 0 warnings, 0 errors, elapsed `00:02:02.45`. |
| `dotnet test .\CanDoItAll.CodeAnalsis.slnx --no-build` | Blocked | Architecture tests passed 8/8, then Unit/Web/Integration VSTest hosts stayed silent for roughly two minutes and were terminated. |
| `.\eng\Validate-SolutionStructure.ps1` | Passed | Ran under Windows PowerShell. |
| `pwsh .\eng\Validate-FileLengths.ps1` | Blocked | `pwsh` is not on PATH. |
| `.\eng\Validate-FileLengths.ps1` | Failed to execute | Windows PowerShell reports `System.IO.Path` has no method `GetRelativePath`. |

## File-Length Hotspots

The existing guardrail declares review threshold 350 and hard max 450 lines in `repo://eng/Validate-FileLengths.ps1`.

| Lines | Path | Classification |
| ---: | --- | --- |
| 801 | `repo://tools/ScenarioEvaluationHarness/Program.cs` | Hard-limit violation, tool monolith. |
| 674 | `repo://tests/CanDoItAll.CodeAnalytics.Tests.Unit/ApplicationFacts.cs` | Hard-limit violation, test monolith. |
| 640 | `repo://src/CanDoItAll.CodeAnalytics.Application/Services/CodeAnalyticsApplicationService.Context.Strategy.cs` | Hard-limit violation, production focused-context strategy monolith. |
| 626 | `repo://tools/ComparisonHarness/Program.cs` | Hard-limit violation, tool monolith. |
| 619 | `repo://src/CanDoItAll.CodeAnalytics.Web/Components/Pages/ContextLab.razor` | Hard-limit violation, desktop sandbox page mixes form, orchestration, rendering, and helpers. |
| 586 | `repo://src/CanDoItAll.CodeAnalytics.Application/Services/CodeAnalyticsApplicationService.Symbols.cs` | Hard-limit violation, symbol query/service shaping monolith. |
| 414 | `repo://src/CanDoItAll.CodeAnalytics.Web/Components/Pages/Snapshots/Context.razor` | Review-threshold warning. |
| 368 | `repo://src/CanDoItAll.CodeAnalytics.Application/Services/CodeAnalyticsApplicationService.Context.SeedResolution.cs` | Review-threshold warning. |
| 358 | `repo://src/CanDoItAll.CodeAnalytics.Web/Components/Pages/Snapshots/Symbols.razor` | Review-threshold warning. |

## Responsibility-Mixing Observations

- `CodeAnalyticsApplicationService` is split into many partial files, but it still owns pipeline orchestration, snapshot querying, symbol search, source file I/O, focused-context seed resolution, scoring, traversal policy, response shaping, and export lookup.
- `ContextLab.razor` mixes UI composition, query binding, snapshot build invocation, focused-context invocation, quality heuristics, rendering, and display helpers.
- `Program.cs` in the Web project performs service registration, endpoint mapping, file-export authorization, settings resolution, workspace picker API handling, and analyze command creation.
- `PersistenceFactCollector` owns project compilation analysis, DbContext discovery, entity resolution, relationship merging, model snapshot handling, diagnostics, and source references.
- Rendering/export code is already isolated from Web, but export orchestration and diagram generation can be made more extensible if the package surface is meant to be public.

## Project-Scale Inventory

| Area | Files | Lines | Max file lines | Notes |
| --- | ---: | ---: | ---: | --- |
| `src/CanDoItAll.CodeAnalytics.Application` | 18 | 4178 | 640 | Primary refactor target. |
| `src/CanDoItAll.CodeAnalytics.Facts` | 26 | 2911 | 234 | Good split by collector, but EF/persistence responsibility is dense. |
| `src/CanDoItAll.CodeAnalytics.Web` | 28 | 2857 | 619 | Desktop sandbox page extraction needed. |
| `src/CanDoItAll.CodeAnalytics.Abstractions` | 61 | 550 | 69 | Many small contracts; candidate for public API polish. |
| `src/CanDoItAll.CodeAnalytics.Rendering` | 6 | 519 | 215 | Candidate for stable rendering package or helper project. |
| `src/CanDoItAll.CodeAnalytics.Storage` | 8 | 230 | 160 | Candidate for filesystem driver split if storage drivers expand. |
| `tools/*` | 2 | 1427 | 801 | Harnesses should be non-shipping and split or documented. |

## Performance Scan Execution Checklist

Production scan scope: `repo://src`, `*.cs`, `*.razor`, excluding `bin` and `obj`.

| Recipe | Hit lines | Triage |
| --- | ---: | --- |
| `async`, `await`, `Task`, `ValueTask` | 187 | Async-heavy app; no immediate issue by itself. |
| `.Result`, `.Wait(`, `GetAwaiter().GetResult()`, `async void` | 4 | False positives on `.Result` property names in search result shaping; no blocking sync-over-async found in production scan. |
| `.IndexOf("` | 0 | No literal overload issue found. |
| `.Substring(` | 0 | No hit. |
| `.StartsWith`, `.EndsWith`, `.Contains` | 128 | Many are collection calls; string calls generally use explicit `StringComparison`. Needs targeted review in hot paths. |
| `.ToLower()` / `.ToUpper()` | 0 | No culture-sensitive allocation pattern found. |
| `.Replace(` | 19 | Mostly path normalization, slug cleanup, and Mermaid escaping; review helper consolidation. |
| `params` | 4 | Low concern; review hot scoring helpers only. |
| LINQ `.Select`, `.Where`, `.OrderBy`, `.GroupBy` | 453 | Significant in query/scoring/rendering paths; benchmark before replacing. |
| `.All` / `.Any` | 26 | Mixed collection use; no blanket issue. |
| `new Dictionary<` / `new List<` | 68 | Expected in collectors and query paths; inspect hot repeated request paths for reusable indexes. |
| `static readonly Dictionary<` | 0 | No FrozenDictionary candidate from static readonly dictionaries. |
| `RegexOptions.Compiled` | 2 | One source-generated regex is fine; one per-query `new Regex(... Compiled ...)` needs review. |
| `new Regex(` | 1 | `repo://src/CanDoItAll.CodeAnalytics.Application/Services/CodeAnalyticsApplicationService.Symbols.Source.cs` line 30. |
| `[GeneratedRegex]` | 1 | `repo://src/CanDoItAll.CodeAnalytics.Domain/Identifiers/StableId.cs`. |
| Public/internal class vs sealed class | 2 / 52 | Most classes are sealed; no broad structural issue. |
| `IEquatable` | 0 | Records dominate immutable facts; no immediate struct-equality issue observed. |
| `JsonSerializer` | 10 | Snapshot serializer centralizes options; no immediate issue. |
| File I/O | 12 | Boundedness and allocation should be reviewed for source excerpts and snapshot storage. |

## Performance Findings To Plan

| ID | Severity | Finding | Evidence | Planned handling |
| --- | --- | --- | --- | --- |
| `PERF-001` | Moderate if hot | Heavy LINQ and dictionary/list allocation in snapshot query, focused-context, collector, and rendering paths. | 453 LINQ hit lines, 68 dictionary/list construction hit lines. | Add benchmark or scenario harness proof before targeted rewrites; prefer precomputed indexes over broad LINQ removal. |
| `PERF-002` | Moderate | User-supplied source search builds a compiled regex per request. | `repo://src/CanDoItAll.CodeAnalytics.Application/Services/CodeAnalyticsApplicationService.Symbols.Source.cs` line 30. | Require timeout/caching/non-compiled strategy decision and adversarial regex proof. |
| `PERF-003` | Info to Moderate | Source excerpt and document source paths read entire files into memory. | `File.ReadAllLinesAsync` and `File.ReadAllTextAsync` in Application service files. | Review for bounded window reading or explicit max-file behavior before public release. |
| `PERF-004` | Info | Path normalization and slug replacement logic is duplicated across layers. | 19 `.Replace(` hits including workspace, facts, storage, web, rendering, and stable-id code. | Consolidate only if it clarifies behavior and avoids accidental path semantics drift. |

## EF Core Query Review

- Production `src/` does not reference `Microsoft.EntityFrameworkCore` packages as an app data access layer.
- EF Core packages are present in `repo://tests/fixtures/Fixture.Shop/src/Fixture.Shop.Infrastructure/Fixture.Shop.Infrastructure.csproj`.
- The app statically analyzes EF concepts through `Facts.Persistence`; it does not execute EF LINQ queries against a production database.
- Fixture query usage is minimal: `EfRepository<TEntity>.FindAsync` and `OrderService.SaveChangesAsync`; no N+1 loop, no read-only query needing `AsNoTracking`, and no `Include`/split-query choice was observed in the fixture.
- Publishing risk is therefore analyzer capability and fixture coverage, not runtime EF query tuning in the app itself.

## Documentation Inventory

| Artifact | Current state | Improvement dependency |
| --- | --- | --- |
| `repo://README.md` | Minimal repo overview and validation commands. | Update after packaging, test, UI, and project extraction decisions. |
| `repo://architecture/adrs/README.md` | Placeholder only. | Add ADRs after subbundles decide public API, project extraction, and driver boundaries. |
| `repo://reference/compatibility-matrix.md` | Useful host integration notes. | Update when driver/addon boundaries are finalized. |
| `repo://reference/tool-surface-proposal.json` | Draft tool surface proposal. | Update after API hardening and final public contracts. |
| `repo://tools/ComparisonHarness/README.md` | Harness-specific run notes. | Split/update after harness refactor or non-shipping decision. |
| Missing docs | License, security, contributing, code of conduct, changelog/release process, package README, public API guide, desktop sandbox guide. | Must follow final package and project layout decisions. |

## Open-Source Packaging Inventory

- Production projects currently have no package metadata such as `PackageId`, `Description`, `RepositoryUrl`, `PackageLicenseExpression`, `PackageReadmeFile`, `PackageTags`, package validation, or central packability policy.
- Test projects mark `IsPackable=false`; production packability needs explicit decisions.
- No license/security/contributing files were found by documentation scan.
