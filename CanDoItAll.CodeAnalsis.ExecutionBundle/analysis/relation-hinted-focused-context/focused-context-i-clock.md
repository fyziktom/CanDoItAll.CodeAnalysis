# Common helper scenario

## Query

- Query text: `IClock`
- Focus tags: None
- Relation hints: None
- Depth: 2
- Requested intent: `Auto`
- Requested precision: `Auto`
- Elapsed milliseconds: 1423

## Resolution

- Seed type: CanDoItAll.SharedKernel.IClock
- Seed member: CanDoItAll.SharedKernel.IClock.GetUtcNow()
- Seed explanation: Resolved from prompt text to member CanDoItAll.SharedKernel.IClock.GetUtcNow().
- Strategy explanation: Auto resolved to surgical definition mode because CanDoItAll.SharedKernel.IClock spans 167 callers across 16 projects. Consumer expansion is capped to direct usages.
- Resolved relation hints: None
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

- Total callers: 167
- Total clusters: 20
- Omitted callers: 50
- Cluster: `CanDoItAll.Modules.Processes` / `CanDoItAll.Modules.Processes`
  Caller count: 46
  Sample: `CanDoItAll.Modules.Processes.ProcessEscalationService` -> `CanDoItAll.Modules.Processes.ProcessEscalationService.AppendEscalationStateAsync(System.Guid, string, CanDoItAll.Modules.Processes.ProcessEscalationStatus, string?, string?, System.Guid?, string, System.Func<CanDoItAll.Modules.Processes.ProcessEscalationViewModel, System.DateTimeOffset, CanDoItAll.Modules.Processes.ProcessDecisionRecord>?, System.Threading.CancellationToken)`
  Path: src/CanDoItAll.Modules.Processes/Runtime/ProcessOperatorControlPlane.cs
  Line: 641
  Reason: ConsumerService sample.
- Cluster: `CanDoItAll.Modules.Workbench` / `CanDoItAll.Modules.Workbench`
  Caller count: 42
  Sample: `CanDoItAll.Modules.Workbench.ProjectGanttPreviewService` -> `CanDoItAll.Modules.Workbench.ProjectGanttPreviewService.BuildAsync(System.Guid, System.Threading.CancellationToken)`
  Path: src/CanDoItAll.Modules.Workbench/Services/ProjectGanttPreviewService.cs
  Line: 10
  Reason: PreviewLifecycle sample.
- Cluster: `CanDoItAll.Modules.CrmHr` / `CanDoItAll.Modules.CrmHr`
  Caller count: 16
  Sample: `CanDoItAll.Modules.CrmHr.AiAgentService` -> `CanDoItAll.Modules.CrmHr.AiAgentService.SaveAgentProfileAsync(CanDoItAll.Modules.CrmHr.AiAgentProfileEditorModel, System.Threading.CancellationToken)`
  Path: src/CanDoItAll.Modules.CrmHr/Services/CrmHrServices.cs
  Line: 4228
  Reason: ConsumerService sample.
- Cluster: `CanDoItAll.Modules.Automation` / `CanDoItAll.Modules.Automation`
  Caller count: 13
  Sample: `CanDoItAll.Modules.Automation.AutomationMessageDispatcher` -> `CanDoItAll.Modules.Automation.AutomationMessageDispatcher.ClaimAndDispatchAsync(System.Guid, System.Threading.CancellationToken)`
  Path: src/CanDoItAll.Modules.Automation/Services/AutomationMessagingServices.cs
  Line: 203
  Reason: ConsumerService sample.

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

