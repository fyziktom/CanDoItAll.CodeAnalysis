# SB-00 Gap analysis and contract design

## Status

- Completed

## Objective

- Freeze the missing SharpTools-style symbol-tool set and the explicit service contracts before implementation starts.

## Covered Inputs

- focus on tools that we are missing and sharptools has them
- analyze what we are missing

## Exact Source References

- C:\repositories\CanDoItAll.CodeAnalsis\CanDoItAll.CodeAnalsis.SharpToolsComparisonBundle\analysis\04-comparison-results.md
- C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Abstractions\ICodeAnalyticsApplicationService.cs
- C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Application\Services\CodeAnalyticsApplicationService.Queries.cs
- C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Web\Components\Pages\Snapshots\Types.razor

## Prerequisites

- The prior comparison bundle is available for reference.
- The current product surface has been inspected.

## Deliverables

- Frozen missing-capability list
- New query and response contract plan
- Search, definition, members, implementations, and references scope limits

## Dependency Impact

- Blocks every implementation subbundle because the contract split determines both the abstractions and the UI workflow.

## Validation Depth

- Bundle doc review
- Contract sanity review against the current snapshot model

## Implementation Steps

1. Freeze the missing SharpTools-style surface that is actually absent in the product.
2. Map each missing capability to the current snapshot facts that can support it.
3. Freeze the explicit query and response contracts and the UI route scope.

## Do Not Do

- Do not turn this phase into implementation.
- Do not assume a second Roslyn query pipeline unless the snapshot model is proven insufficient.

## Acceptance Checklist

- The missing capability list is explicit.
- The planned contracts are explicit.
- The scope limits are explicit.

## Proof Required

- Updated bundle analysis notes
- Updated phase plan and traceability

## Browser Validation Logging

- N/A in this phase

## Progression Gate

- Downstream implementation may continue only when the missing capability list and contract boundary are both explicit.

## Suggested Agent Prompt

Freeze the symbol-tools parity surface first. Confirm exactly which SharpTools-style capabilities are still missing, map them to the existing snapshot facts, and lock the contract split before touching implementation code.
