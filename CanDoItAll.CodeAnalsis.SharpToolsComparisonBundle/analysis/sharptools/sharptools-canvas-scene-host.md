# UI scenario

## Query

- Query text: `CanvasSceneHost`
- Warm elapsed milliseconds: 25376
- Warm call count: 4

## Search sequence

1. `SearchDefinitions("\\bCanvasSceneHost\\b")`
   Result: exact class definition and one instantiation inside the preview factory. No noisy refinement was needed.
2. `ViewDefinition("CanDoItAll.Components.CanvasLib.CanvasSceneHost")`
3. `GetMembers("CanDoItAll.Components.CanvasLib.CanvasSceneHost", includePrivateMembers: false)`
4. `FindReferences("CanDoItAll.Components.CanvasLib.CanvasSceneHost")`

## Definition

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

`GetMembers` returned four properties and five methods:

- `IsInitialized`
- `SurfaceSyncPending`
- `PendingStateKey`
- `AppliedStateKey`
- `QueueSync(string?, bool)`
- `ShouldCreate()`
- `ShouldUpdate()`
- `MarkApplied()`
- `Reset()`

## References

`FindReferences` reported exactly `1` reference:

- `CanDoItAll.Components.CanvasLib.CanvasSceneHostPreviewFactory.CreateForWorkbench(CanvasWorkbenchSurface)`
  Context: `var host = new CanvasSceneHost(); host.QueueSync(stateKey, true); var createPath = host.ShouldCreate();`

## Working impression

This is an excellent SharpTools case. The class is small, the definition is the real behavior, and the single reference immediately shows how the host participates in the preview lifecycle. Very little follow-up is needed.
