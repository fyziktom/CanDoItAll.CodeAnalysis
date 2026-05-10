# cando-canvas-mark-applied

## Simulated Prompt

A canvas dirty-state bug points at CanvasSceneHost.MarkApplied. Show that member and directly related invalidation state.

## Simulated Agent Approach

Ask focused context for the named host with relation hints around MarkApplied and invalidation.

## Query

- Repository: `CanDoItAll`
- Category: `Specific`
- Query text: `MarkApplied`
- Focus tags: `Ui`
- Relation hints: `CanvasSceneHost`, `InvalidationScheduler`
- Depth: 2
- Intent: `Auto`
- Precision: `Auto`

## Score

- Rating: `Good`
- Helpfulness score: 0,873
- Expected terms: 2/3
- Expected files: 2/2
- Useful files: 3
- Non-useful files: 0
- Noise term hits: 0
- Token budget ratio: 0,538

## Output Metrics

- Search results: 1
- Seed type: CanDoItAll.Components.CanvasLib.CanvasSceneHost
- Seed member: CanDoItAll.Components.CanvasLib.CanvasSceneHost.MarkApplied()
- Files: 3
- Blocks: 6
- Selected lines: 59
- Estimated tokens: 1184
- Usage callers: None
- Usage clusters: None

## Symbol Search Top Results

- `CanDoItAll.Components.CanvasLib.CanvasSceneHost.MarkApplied()` (Member)

## Selected Files

- `src/CanDoItAll.Components.CanvasLib/Canvas/Core/CanvasSceneHost.cs`: 52/91 lines, 4 blocks
- `src/CanDoItAll.Components.CanvasLib/Canvas/Core/SerializationPersistencePack.cs`: 4/73 lines, 1 blocks
- `src/CanDoItAll.Components.CanvasLib/Canvas/Workbench/CanvasWorkbenchSurface.cs`: 3/33 lines, 1 blocks
