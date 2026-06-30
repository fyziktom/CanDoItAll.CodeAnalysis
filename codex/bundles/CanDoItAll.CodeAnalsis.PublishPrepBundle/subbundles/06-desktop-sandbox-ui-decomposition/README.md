# Desktop Sandbox UI Decomposition

## Status

- `Completed`

## Objective

- Split large desktop sandbox Razor pages into maintainable components while preserving large-screen workflows.

## Success Criteria

- Large UI files are decomposed into subcomponents with clear responsibilities.
- Desktop-large browser proof confirms key workflows are readable, unclipped, and functional.
- No effort is spent polishing small or medium responsive layouts beyond avoiding regressions introduced by the refactor.

## Covered Inputs

- `IN-003`
- `IN-006`
- `REQ-008`

## Prerequisites

- `SB01` validation baseline passed.
- `SB03` application response behavior stabilized.
- If `SB05` changes export/storage routes, include those changes in smoke proof.

## Exact Source References

- `repo://src/CanDoItAll.CodeAnalytics.Web/Components/Pages/Home.razor`
- `repo://src/CanDoItAll.CodeAnalytics.Web/Components/Pages/ContextLab.razor`
- `repo://src/CanDoItAll.CodeAnalytics.Web/Components/Pages/Operations/Details.razor`
- `repo://src/CanDoItAll.CodeAnalytics.Web/Components/Pages/Snapshots/Dashboard.razor`
- `repo://src/CanDoItAll.CodeAnalytics.Web/Components/Pages/Snapshots/Context.razor`
- `repo://src/CanDoItAll.CodeAnalytics.Web/Components/Pages/Snapshots/Symbols.razor`
- `repo://src/CanDoItAll.CodeAnalytics.Web/Components/Shared/StatCard.razor`
- `repo://src/CanDoItAll.CodeAnalytics.Web/Components/Shared/SnapshotTabs.razor`
- `repo://src/CanDoItAll.CodeAnalytics.Web/wwwroot/styles/base.css`
- `repo://src/CanDoItAll.CodeAnalytics.Web/wwwroot/styles/snapshots.css`
- `repo://src/CanDoItAll.CodeAnalytics.Web/wwwroot/workspace-picker.js`

## Deliverables

- Extracted components for Context Lab form, run summary, selected files, usage summary, supporting context, symbol search results, definition/details, members, implementations, references.
- Page files below the agreed line threshold or documented exceptions.
- Large-screen Playwright/browser evidence and screenshots.
- Web tests or component/render tests for route behavior where feasible.

## Dependency Impact

- `SB07` and `SB08` depend on UI behavior and screenshots/docs matching the final desktop sandbox.

## Validation Depth

- UI and component-test proof with large-screen browser validation.

## Implementation Steps

1. Re-run file-length inventory after prior subbundles.
2. Extract repeated panels/cards/lists/details into components using existing styles.
3. Keep forms and query parameters behavior-compatible.
4. Run Web tests and build.
5. Start the app and validate large desktop routes with Playwright/browser proof.
6. Capture screenshots and answer visual review questions in execution report.

## Scope Exceptions

- Small/medium responsive tuning is out of scope by user instruction.
- Do not redesign the sandbox as a marketing site.

## Do Not Do

- Do not change application-service behavior here.
- Do not introduce a new UI framework unless an explicit blocker exists.
- Do not create nested card-in-card layouts or large decorative pages; keep dense desktop scanning useful.

## Acceptance Checklist

- ContextLab, Context, and Symbols pages are below threshold or have accepted exceptions.
- Components have clear names and responsibility boundaries.
- Desktop-large route proof covers home, context lab, operation details, dashboard, context, symbols, and exports/persistence where changed.
- Screenshots show readable text, no clipping, no incoherent overlap, and stable action placement.
- Build, Web tests, validation matrix, and file-length validation pass.

## Proof Required

- Build/Web test transcripts.
- Browser proof at `>=1600x900` desktop-large viewport.
- Screenshots under `bundle://proof/SB06/browser/`.
- Visual review answers in `reviews/01-execution-report.md`.
- Source assertions for component boundaries.
- Anti-stub audit transcript.

## Browser Validation Logging

- Routes: `/`, `/context-lab`, `/operations/{id}`, `/snapshots/{id}`, `/snapshots/{id}/context`, `/snapshots/{id}/symbols`, and changed snapshot tabs.
- Viewport: large desktop, recommended `1600x1000` or maximized equivalent.
- Actions: navigate, submit safe forms where possible, open details/accordions, inspect long code blocks, validate tabs/links, test workspace picker failure/cancel-safe UI if host picker cannot be opened.
- Screenshots: save before/after or final route screenshots under `bundle://proof/SB06/browser/`.
- Review questions: text readable, no clipping, no overlap, no excessive empty desktop space, dense data scannable, controls discoverable.

## Progression Gate

- `SB07` and `SB08` may proceed only after desktop-large browser proof passes for changed UI flows.

## Suggested Agent Prompt

```text
Implement SB06 only. Decompose the desktop sandbox UI, preserve large-screen workflows, capture desktop browser proof, and do not spend time on small/medium responsive tuning.
```
