# UI scenario

## Query

- Query text: `CanvasSceneHost`
- Focus tags: `Ui`
- Relation hints: None
- Depth: 2
- Requested intent: `Auto`
- Requested precision: `Auto`
- Elapsed milliseconds: 1341

## Resolution

- Seed type: CanDoItAll.Components.CanvasLib.CanvasSceneHost
- Seed member: CanDoItAll.Components.CanvasLib.CanvasSceneHost.MarkApplied()
- Seed explanation: Resolved from prompt text to member CanDoItAll.Components.CanvasLib.CanvasSceneHost.MarkApplied().
- Strategy explanation: Used default trouble-path expansion.
- Resolved relation hints: None
- Resolved intent: `TroublePath`
- Resolved precision: `Balanced`

## Stats

- Files: 3
- Blocks: 6
- Selected lines: 59
- Total lines in selected files: 197

## Selection Reasons

- `CanDoItAll.Components.CanvasLib.CanvasSceneHostPreviewFactory.CreateForWorkbench(CanDoItAll.Components.CanvasLib.CanvasWorkbenchSurface)`
  Target kind: `Member`
  Reason: `RelatedContext`
  Role: `Factory`
- `CanDoItAll.Components.CanvasLib.CanvasSceneHost.ShouldCreate()`
  Target kind: `Member`
  Reason: `SeedContext`
  Role: `PreviewLifecycle`
- `CanDoItAll.Components.CanvasLib.CanvasSceneHost.MarkApplied()`
  Target kind: `Member`
  Reason: `Seed`
  Role: `PreviewLifecycle`
- `CanDoItAll.Components.CanvasLib.CanvasSceneHost.QueueSync(string?, bool)`
  Target kind: `Member`
  Reason: `SeedContext`
  Role: `PreviewLifecycle`
- `CanDoItAll.Components.CanvasLib.SerializationPersistencePack.Serialize<T>(T)`
  Target kind: `Member`
  Reason: `RelatedContext`
  Role: `PreviewLifecycle`
- `src/CanDoItAll.Components.CanvasLib/Canvas/Core/CanvasSceneHost.cs`
  Target kind: `File`
  Reason: `Seed`
  Role: `PreviewLifecycle`
- `src/CanDoItAll.Components.CanvasLib/Canvas/Core/CanvasSceneHost.cs`
  Target kind: `File`
  Reason: `SeedContext`
  Role: `PreviewLifecycle`
- `src/CanDoItAll.Components.CanvasLib/Canvas/Core/CanvasSceneHost.cs`
  Target kind: `File`
  Reason: `RelatedContext`
  Role: `Factory`
- `src/CanDoItAll.Components.CanvasLib/Canvas/Core/SerializationPersistencePack.cs`
  Target kind: `File`
  Reason: `RelatedContext`
  Role: `PreviewLifecycle`
- `src/CanDoItAll.Components.CanvasLib/Canvas/Workbench/CanvasWorkbenchSurface.cs`
  Target kind: `File`
  Reason: `Implementation`
  Role: `PreviewLifecycle`

## Implementation Types

- None

## Selected Types

- `CanDoItAll.Components.CanvasLib.CanvasSceneHost`
  Path: src/CanDoItAll.Components.CanvasLib/Canvas/Core/CanvasSceneHost.cs
  Kind: `Class`
  Project: `proj-candoitall-components-canvaslib`
- `CanDoItAll.Components.CanvasLib.CanvasSceneHostPreviewFactory`
  Path: src/CanDoItAll.Components.CanvasLib/Canvas/Core/CanvasSceneHost.cs
  Kind: `Class`
  Project: `proj-candoitall-components-canvaslib`
- `CanDoItAll.Components.CanvasLib.SerializationPersistencePack`
  Path: src/CanDoItAll.Components.CanvasLib/Canvas/Core/SerializationPersistencePack.cs
  Kind: `Class`
  Project: `proj-candoitall-components-canvaslib`
- `CanDoItAll.Components.CanvasLib.CanvasWorkbenchSurface`
  Path: src/CanDoItAll.Components.CanvasLib/Canvas/Workbench/CanvasWorkbenchSurface.cs
  Kind: `Class`
  Project: `proj-candoitall-components-canvaslib`
- `CanDoItAll.Components.CanvasLib.CanvasSceneHostPreviewSnapshot`
  Path: src/CanDoItAll.Components.CanvasLib/Canvas/Core/CanvasSceneHost.cs
  Kind: `Class`
  Project: `proj-candoitall-components-canvaslib`

## Selected Members

- `CanDoItAll.Components.CanvasLib.CanvasSceneHost.MarkApplied()`
  Type: `CanDoItAll.Components.CanvasLib.CanvasSceneHost`
  Kind: `Method`
  Path: src/CanDoItAll.Components.CanvasLib/Canvas/Core/CanvasSceneHost.cs
  Line: 23
- `CanDoItAll.Components.CanvasLib.CanvasSceneHost.QueueSync(string?, bool)`
  Type: `CanDoItAll.Components.CanvasLib.CanvasSceneHost`
  Kind: `Method`
  Path: src/CanDoItAll.Components.CanvasLib/Canvas/Core/CanvasSceneHost.cs
  Line: 13
- `CanDoItAll.Components.CanvasLib.CanvasSceneHost.ShouldCreate()`
  Type: `CanDoItAll.Components.CanvasLib.CanvasSceneHost`
  Kind: `Method`
  Path: src/CanDoItAll.Components.CanvasLib/Canvas/Core/CanvasSceneHost.cs
  Line: 19
- `CanDoItAll.Components.CanvasLib.CanvasSceneHostPreviewFactory.CreateForWorkbench(CanDoItAll.Components.CanvasLib.CanvasWorkbenchSurface)`
  Type: `CanDoItAll.Components.CanvasLib.CanvasSceneHostPreviewFactory`
  Kind: `Method`
  Path: src/CanDoItAll.Components.CanvasLib/Canvas/Core/CanvasSceneHost.cs
  Line: 56
- `CanDoItAll.Components.CanvasLib.SerializationPersistencePack.Serialize<T>(T)`
  Type: `CanDoItAll.Components.CanvasLib.SerializationPersistencePack`
  Kind: `Method`
  Path: src/CanDoItAll.Components.CanvasLib/Canvas/Core/SerializationPersistencePack.cs
  Line: 12

## Usage Summary

- None

## File Excerpts

### src/CanDoItAll.Components.CanvasLib/Canvas/Core/CanvasSceneHost.cs

- Total lines: 91
- Selected lines: 52
- Types: `CanDoItAll.Components.CanvasLib.CanvasSceneHost`, `CanDoItAll.Components.CanvasLib.CanvasSceneHostPreviewFactory`, `CanDoItAll.Components.CanvasLib.CanvasSceneHostPreviewSnapshot`

#### CanDoItAll.Components.CanvasLib.CanvasSceneHost.MarkApplied()

- Kind: `Method`
- Lines: 22-29

```csharp

    public void MarkApplied()
    {
        AppliedStateKey = PendingStateKey;
        SurfaceSyncPending = false;
        IsInitialized = true;
    }

```

#### CanDoItAll.Components.CanvasLib.CanvasSceneHost.QueueSync(string?, bool)

- Kind: `Method`
- Lines: 12-18

```csharp

    public void QueueSync(string? stateKey, bool shouldSync = true)
    {
        PendingStateKey = stateKey;
        SurfaceSyncPending = shouldSync;
    }

```

#### CanDoItAll.Components.CanvasLib.CanvasSceneHost.ShouldCreate()

- Kind: `Method`
- Lines: 18-20

```csharp

    public bool ShouldCreate() => !IsInitialized;

```

#### CanDoItAll.Components.CanvasLib.CanvasSceneHostPreviewFactory.CreateForWorkbench(CanDoItAll.Components.CanvasLib.CanvasWorkbenchSurface)

- Kind: `Method`
- Lines: 55-89

```csharp
{
    public static CanvasSceneHostPreviewSnapshot CreateForWorkbench(CanvasWorkbenchSurface surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        var stateKey = SerializationPersistencePack.Serialize(new
        {
            surface.SurfaceId,
            NodeCount = surface.Nodes.Count,
            LinkCount = surface.Links.Count,
            Selected = surface.UiState.SelectedNodeIds.Count
        });

        var host = new CanvasSceneHost();
        host.QueueSync(stateKey, true);
        var createPath = host.ShouldCreate();
        host.MarkApplied();
        host.QueueSync($"{stateKey}:update", true);
        var updatePath = host.ShouldUpdate();

        return new CanvasSceneHostPreviewSnapshot
        {
            Title = "Scene host tracks create and update sync without page-specific state flags",
            Summary = "The shared host now owns pending state keys, applied keys, and initialization transitions so every preview boundary can follow the same create-update lifecycle.",
            StatePill = host.IsInitialized ? "Synced" : "Pending",
            Metrics =
            [
                createPath ? "Create path armed" : "Create path idle",
                updatePath ? "Update path armed" : "Update path idle",
                $"{surface.Nodes.Count} nodes mirrored",
                host.SurfaceSyncPending ? "Sync pending" : "Sync settled"
            ]
        };
    }
}
```

### src/CanDoItAll.Components.CanvasLib/Canvas/Core/SerializationPersistencePack.cs

- Total lines: 73
- Selected lines: 4
- Types: `CanDoItAll.Components.CanvasLib.SerializationPersistencePack`

#### CanDoItAll.Components.CanvasLib.SerializationPersistencePack.Serialize<T>(T)

- Kind: `Method`
- Lines: 11-14

```csharp

    public static string Serialize<T>(T value)
        => JsonSerializer.Serialize(value, DefaultOptions);

```

### src/CanDoItAll.Components.CanvasLib/Canvas/Workbench/CanvasWorkbenchSurface.cs

- Total lines: 33
- Selected lines: 3
- Types: `CanDoItAll.Components.CanvasLib.CanvasWorkbenchSurface`

#### CanDoItAll.Components.CanvasLib.CanvasWorkbenchSurface.UiState

- Kind: `Property`
- Lines: 14-16

```csharp

    public CanvasWorkbenchUiState UiState { get; set; } = new();

```

