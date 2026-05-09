# Common helper with relation hint

## Query

- Query text: `IClock`
- Focus tags: None
- Relation hints: `Workbench`
- Depth: 2
- Requested intent: `Auto`
- Requested precision: `Auto`
- Elapsed milliseconds: 1302

## Resolution

- Seed type: CanDoItAll.SharedKernel.IClock
- Seed member: CanDoItAll.SharedKernel.IClock.GetUtcNow()
- Seed explanation: Resolved from prompt text to member CanDoItAll.SharedKernel.IClock.GetUtcNow().
- Strategy explanation: Auto resolved to surgical definition mode because CanDoItAll.SharedKernel.IClock spans 167 callers across 16 projects. Consumer expansion is capped to direct usages. Relation hints constrain representative usage sampling.
- Resolved relation hints: `workbench`
- Resolved intent: `Definition`
- Resolved precision: `Surgical`

## Stats

- Files: 1
- Blocks: 2
- Selected lines: 6
- Total lines in selected files: 11

## Selection Reasons

- `CanDoItAll.SharedKernel.SystemClock.GetUtcNow()`
  Target kind: `Member`
  Reason: `Implementation`
  Role: `None`
- `CanDoItAll.SharedKernel.IClock.GetUtcNow()`
  Target kind: `Member`
  Reason: `Seed`
  Role: `None`
- `src/CanDoItAll.SharedKernel/Time/IClock.cs`
  Target kind: `File`
  Reason: `Seed`
  Role: `None`
- `src/CanDoItAll.SharedKernel/Time/IClock.cs`
  Target kind: `File`
  Reason: `Implementation`
  Role: `None`

## Implementation Types

- `CanDoItAll.SharedKernel.SystemClock`
  Path: src/CanDoItAll.SharedKernel/Time/IClock.cs
  Kind: `Class`
  Project: `proj-candoitall-sharedkernel`

## Selected Types

- `CanDoItAll.SharedKernel.IClock`
  Path: src/CanDoItAll.SharedKernel/Time/IClock.cs
  Kind: `Interface`
  Project: `proj-candoitall-sharedkernel`
- `CanDoItAll.SharedKernel.SystemClock`
  Path: src/CanDoItAll.SharedKernel/Time/IClock.cs
  Kind: `Class`
  Project: `proj-candoitall-sharedkernel`

## Selected Members

- `CanDoItAll.SharedKernel.IClock.GetUtcNow()`
  Type: `CanDoItAll.SharedKernel.IClock`
  Kind: `Method`
  Path: src/CanDoItAll.SharedKernel/Time/IClock.cs
  Line: 5
- `CanDoItAll.SharedKernel.SystemClock.GetUtcNow()`
  Type: `CanDoItAll.SharedKernel.SystemClock`
  Kind: `Method`
  Path: src/CanDoItAll.SharedKernel/Time/IClock.cs
  Line: 10

## Usage Summary

- Total callers: 42
- Total clusters: 1
- Omitted callers: 0
- Cluster: `CanDoItAll.Modules.Workbench` / `CanDoItAll.Modules.Workbench`
  Caller count: 42
  Sample: `CanDoItAll.Modules.Workbench.ProjectGanttPreviewService` -> `CanDoItAll.Modules.Workbench.ProjectGanttPreviewService.BuildAsync(System.Guid, System.Threading.CancellationToken)`
  Path: src/CanDoItAll.Modules.Workbench/Services/ProjectGanttPreviewService.cs
  Line: 10
  Reason: PreviewLifecycle sample.

## File Excerpts

### src/CanDoItAll.SharedKernel/Time/IClock.cs

- Total lines: 11
- Selected lines: 6
- Types: `CanDoItAll.SharedKernel.IClock`, `CanDoItAll.SharedKernel.SystemClock`

#### CanDoItAll.SharedKernel.IClock.GetUtcNow()

- Kind: `Method`
- Lines: 4-6

```csharp
{
    DateTimeOffset GetUtcNow();
}
```

#### CanDoItAll.SharedKernel.SystemClock.GetUtcNow()

- Kind: `Method`
- Lines: 9-11

```csharp
{
    public DateTimeOffset GetUtcNow() => DateTimeOffset.UtcNow;
}
```

