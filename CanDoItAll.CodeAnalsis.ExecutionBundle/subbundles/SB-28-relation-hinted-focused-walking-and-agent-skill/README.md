# SB-28 Relation-hinted focused walking and agent skill

## Status

- Completed

Completion date: 2026-05-09.

## Objective

Add an explicit relation-hint axis to focused context so agents can ask for a seed symbol plus related functions, classes, components, or architectural areas, then receive bounded usage context that is narrower than a broad tag-only or whole-solution scan.

## Covered Inputs

- `inputs/07-relation-hinted-walking-follow-up.md`

## Prerequisites

- SB-24 through SB-27 remain trusted.
- Existing focused-context query supports tags, depth, intent, precision, selection reasons, and lab rendering.
- Existing host MCP driver already wraps `FocusedContextQuery` through `CodeAnalyticsFocusedContextInput`.

## Exact Source References

- C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Abstractions\Queries\FocusedContextQuery.cs
- C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Abstractions\Responses\FocusedContextResponse.cs
- C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Application\Services\CodeAnalyticsApplicationService.Context.cs
- C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Application\Services\CodeAnalyticsApplicationService.Context.Strategy.cs
- C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Application\Services\CodeAnalyticsApplicationService.Context.FocusTags.cs
- C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Web\Components\Pages\ContextLab.razor
- C:\repositories\CanDoItAll.CodeAnalsis\tools\ComparisonHarness\Program.cs
- C:\repositories\CanDoItAll.CodeAnalsis\tests\CanDoItAll.CodeAnalytics.Tests.Unit\ApplicationFacts.cs
- C:\repositories\CanDoItAll.CodeAnalsis\tests\CanDoItAll.CodeAnalytics.Tests.Web\WebUiFacts.cs
- C:\repositories\CanDoItAll\src\CanDoItAll.Mcp.CodeAnalytics\CodeAnalyticsModels.cs
- C:\repositories\CanDoItAll\src\CanDoItAll.Mcp.CodeAnalytics\CodeAnalyticsCoordinator.cs
- C:\repositories\CanDoItAll\codex\skills\candoitall-codeanalytics-mcp\SKILL.md

## Deliverables

- `FocusedContextQuery` and `FocusedContextResponse` carry normalized relation hints.
- Helper usage-summary clustering filters or strongly biases representative consumers by relation hints when they are supplied.
- Tag aliases include practical persistence terms such as `EntityFramework` and `EFCore`.
- `/context-lab` exposes relation hints and renders the resolved hints next to tags and strategy metadata.
- Host MCP input model exposes relation hints to agents.
- The CanDoItAll codeanalytics MCP skill explains scoped snapshot use, exact symbol first, and relation-hinted focused walking.
- Unit, web, and harness evidence quantify selected files, selected lines, usage clusters, omitted callers, characters, and estimated tokens.

## Dependency Impact

- This is a critical focused-context tuning subbundle. A bad relation-hint implementation would undermine the main value proposition of saving context for large projects.
- Host MCP wrappers must compile against the changed abstraction contract.
- The skill guidance must match the actual MCP input model so agents do not learn a parameter that the server cannot accept.

## Validation Depth

- Unit tests for relation-hinted helper sampling.
- Web test for lab route parameter binding and visible relation-hint metadata.
- Build and test the standalone solution.
- Build the host `CanDoItAll.Mcp.CodeAnalytics` project to prove MCP wrapper compatibility.
- Run the comparison harness against `C:\repositories\CanDoItAll\CanDoItAll.slnx` and record metrics.
- Browser proof for `/context-lab` with relation hints.

## Implementation Steps

1. Add relation hints to the focused-context query and response contract without breaking existing callers.
2. Normalize relation hints with the same delimiter behavior as tags.
3. Use relation hints in representative-consumer scoring and filtering so helper usage summaries can be narrowed to related classes, functions, components, namespaces, project names, or paths.
4. Extend the lab UI and host MCP input mapping.
5. Update the agent skill with explicit advanced-walking sequences.
6. Extend tests and harness metrics.
7. Rerun validation and write quantitative evidence back into the bundle.

## Scope Exceptions

- This subbundle does not implement SharpTools write operations, automatic source edits, or Git-backed undo.
- This subbundle does not build a persistent semantic tag taxonomy. Tags and relation hints remain explicit request inputs.
- This subbundle does not change the default full-solution snapshot command because scoped snapshots already exist; guidance and UI must make the scoped path obvious.

## Do Not Do

- Do not silently fall back to broad representative consumers when relation hints are provided but do not match.
- Do not hide relation hints only in UI state; they must be part of the application and MCP contracts.
- Do not weaken the existing exact symbol tools by routing exact definition questions through focused context first.
- Do not edit unrelated CanDoItAll host files that are already dirty in the current worktree.

## Acceptance Checklist

- Relation hints are normalized, returned, and visible in response metadata.
- A high-fan-in helper plus relation hint returns usage clusters limited to matching related context.
- Existing tag-only and no-hint focused-context tests still pass.
- The lab accepts `relationHints` in the URL and displays the resolved hints.
- The host MCP input model and coordinator pass relation hints into the engine.
- The codeanalytics MCP skill tells agents when to use scoped snapshots, exact symbol tools, focused context, tags, relation hints, and depth.
- Harness output includes at least one relation-hinted scenario and quantified token or line savings.

## Proof Required

- `dotnet build C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.slnx -nologo` passed with 0 warnings and 0 errors.
- `dotnet test C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.slnx --no-build -nologo` passed 70 tests.
- `dotnet build C:\repositories\CanDoItAll\src\CanDoItAll.Mcp.CodeAnalytics\CanDoItAll.Mcp.CodeAnalytics.csproj -nologo` passed with 0 warnings and 0 errors.
- Comparison harness output was written under `CanDoItAll.CodeAnalsis.ExecutionBundle\analysis\relation-hinted-focused-context`.
- Browser screenshots were written under `output\playwright`.

## Browser Validation Logging

- Route: `/context-lab`
- Query: fixture solution path, `queryText=PlaceOrderAsync`, `tags=EntityFramework`, `relationHints=OrderService`, `depth=2`.
- Viewport: `1440x1200` and `390x1000`.
- Required checks passed: relation-hints input visible, run summary displays normalized relation hints, selected files and supporting context remain readable, no overlapping text.

## Progression Gate

- Passed. Code, UI, MCP wrapper, skill, tests, browser proof, and harness metrics agree that relation hints narrow helper context without regressing existing focused-context flows.

## Quantified Result

- Host snapshot: 67 projects, 3731 types, 31045 members.
- Plain helper scenario `IClock`: 1 file, 2 blocks, 6 selected lines, 20 usage clusters, 167 callers, 4724 characters, estimated 1181 tokens.
- Relation-hinted helper scenario `IClock` plus `Workbench`: 1 file, 2 blocks, 6 selected lines, 1 usage cluster, 42 callers, 3283 characters, estimated 821 tokens.
- Savings: usage clusters reduced 95%, callers shown reduced 74.9%, and estimated tokens reduced 30.5% while preserving the exact helper definition excerpt.

## Suggested Agent Prompt

Implement relation-hinted focused walking for focused context. Keep the change small and typed: add relation hints to the query/response contract, use them to narrow helper representative consumers, expose them in the lab and MCP input model, update the agent skill guidance, and prove the improvement with unit tests, web tests, host MCP build, comparison harness metrics, and browser proof.
