# UI scenario

## Query

- Query text: `CanvasSceneHost`
- Elapsed milliseconds: 2087
- Search results: 1
- Selected symbol: `CanDoItAll.Components.CanvasLib.CanvasSceneHost`
- Target kind: `Type`

## Definition

- Declaration: `class CanDoItAll.Components.CanvasLib.CanvasSceneHost`
- Path: src/CanDoItAll.Components.CanvasLib/Canvas/Core/CanvasSceneHost.cs:3
- Truncated: False

```csharp
public sealed class CanvasSceneHost
{
    public bool IsInitialized { get; private set; }

    public bool SurfaceSyncPending { get; private set; } = true;

    public string? PendingStateKey { get; private set; }

    public string? AppliedStateKey { get; private set; }

    public void QueueSync(string? stateKey, bool shouldSync = true)
    {
        PendingStateKey = stateKey;
        SurfaceSyncPending = shouldSync;
    }

    public bool ShouldCreate() => !IsInitialized;

    public bool ShouldUpdate() => IsInitialized && SurfaceSyncPending;

    public void MarkApplied()
    {
        AppliedStateKey = PendingStateKey;
        SurfaceSyncPending = false;
        IsInitialized = true;
    }

    public void Reset()
    {
        PendingStateKey = null;
        AppliedStateKey = null;
        SurfaceSyncPending = true;
        IsInitialized = false;
    }
}
```

## Members

- Member count: 9
- `CanDoItAll.Components.CanvasLib.CanvasSceneHost.MarkApplied()` (Method)
- `CanDoItAll.Components.CanvasLib.CanvasSceneHost.QueueSync(string?, bool)` (Method)
- `CanDoItAll.Components.CanvasLib.CanvasSceneHost.Reset()` (Method)
- `CanDoItAll.Components.CanvasLib.CanvasSceneHost.ShouldCreate()` (Method)
- `CanDoItAll.Components.CanvasLib.CanvasSceneHost.ShouldUpdate()` (Method)
- `CanDoItAll.Components.CanvasLib.CanvasSceneHost.AppliedStateKey` (Property)
- `CanDoItAll.Components.CanvasLib.CanvasSceneHost.IsInitialized` (Property)
- `CanDoItAll.Components.CanvasLib.CanvasSceneHost.PendingStateKey` (Property)
- `CanDoItAll.Components.CanvasLib.CanvasSceneHost.SurfaceSyncPending` (Property)

## Implementations

- Count: 0

## References

- Total references: 4
- Returned references: 4
- `CanDoItAll.Components.CanvasLib.CanvasSceneHostPreviewFactory` :: `CanDoItAll.Components.CanvasLib.CanvasSceneHostPreviewFactory.CreateForWorkbench(CanDoItAll.Components.CanvasLib.CanvasWorkbenchSurface)` (Invocation)
  Path: src/CanDoItAll.Components.CanvasLib/Canvas/Core/CanvasSceneHost.cs:68
- `CanDoItAll.Components.CanvasLib.CanvasSceneHostPreviewFactory` :: `CanDoItAll.Components.CanvasLib.CanvasSceneHostPreviewFactory.CreateForWorkbench(CanDoItAll.Components.CanvasLib.CanvasWorkbenchSurface)` (Invocation)
  Path: src/CanDoItAll.Components.CanvasLib/Canvas/Core/CanvasSceneHost.cs:69
- `CanDoItAll.Components.CanvasLib.CanvasSceneHostPreviewFactory` :: `CanDoItAll.Components.CanvasLib.CanvasSceneHostPreviewFactory.CreateForWorkbench(CanDoItAll.Components.CanvasLib.CanvasWorkbenchSurface)` (Invocation)
  Path: src/CanDoItAll.Components.CanvasLib/Canvas/Core/CanvasSceneHost.cs:70
- `CanDoItAll.Components.CanvasLib.CanvasSceneHostPreviewFactory` :: `CanDoItAll.Components.CanvasLib.CanvasSceneHostPreviewFactory.CreateForWorkbench(CanDoItAll.Components.CanvasLib.CanvasWorkbenchSurface)` (Invocation)
  Path: src/CanDoItAll.Components.CanvasLib/Canvas/Core/CanvasSceneHost.cs:72
