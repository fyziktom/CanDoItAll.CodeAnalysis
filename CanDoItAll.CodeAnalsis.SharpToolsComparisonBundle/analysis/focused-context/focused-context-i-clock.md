# Common helper scenario

## Query

- Query text: `IClock`
- Focus tags: None
- Depth: 2
- Requested intent: `Auto`
- Requested precision: `Auto`
- Elapsed milliseconds: 469

## Resolution

- Seed type: CanDoItAll.SharedKernel.IClock
- Seed member: CanDoItAll.SharedKernel.IClock.GetUtcNow()
- Seed explanation: Resolved from prompt text to member CanDoItAll.SharedKernel.IClock.GetUtcNow().
- Strategy explanation: Auto resolved to surgical definition mode because CanDoItAll.SharedKernel.IClock spans 111 callers across 13 projects. Consumer expansion is capped to direct usages.
- Resolved intent: `Definition`
- Resolved precision: `Surgical`

## Stats

- Files: 4
- Blocks: 6
- Selected lines: 209
- Total lines in selected files: 5756

## Implementation Types

- `CanDoItAll.SharedKernel.SystemClock`
  Path: src/CanDoItAll.SharedKernel/IClock.cs
  Kind: `Class`
  Project: `proj-candoitall-sharedkernel`

## Selected Types

- `CanDoItAll.SharedKernel.IClock`
  Path: src/CanDoItAll.SharedKernel/IClock.cs
  Kind: `Interface`
  Project: `proj-candoitall-sharedkernel`
- `CanDoItAll.SharedKernel.SystemClock`
  Path: src/CanDoItAll.SharedKernel/IClock.cs
  Kind: `Class`
  Project: `proj-candoitall-sharedkernel`
- `CanDoItAll.Modules.Automation.AutomationMessageDispatcher`
  Path: src/CanDoItAll.Modules.Automation/AutomationMessagingServices.cs
  Kind: `Class`
  Project: `proj-candoitall-modules-automation`
- `CanDoItAll.Modules.CrmHr.AiAgentService`
  Path: src/CanDoItAll.Modules.CrmHr/CrmHrCrossModuleIntegration.cs
  Kind: `Class`
  Project: `proj-candoitall-modules-crmhr`
- `CanDoItAll.Modules.Workbench.ProjectCrossModuleMutationCoordinator`
  Path: src/CanDoItAll.Modules.Workbench/ProjectWorkbenchCrossModuleMutations.cs
  Kind: `Class`
  Project: `proj-candoitall-modules-workbench`

## Selected Members

- `CanDoItAll.SharedKernel.IClock.GetUtcNow()`
  Type: `CanDoItAll.SharedKernel.IClock`
  Kind: `Method`
  Path: src/CanDoItAll.SharedKernel/IClock.cs
  Line: 5
- `CanDoItAll.SharedKernel.SystemClock.GetUtcNow()`
  Type: `CanDoItAll.SharedKernel.SystemClock`
  Kind: `Method`
  Path: src/CanDoItAll.SharedKernel/IClock.cs
  Line: 10
- `CanDoItAll.Modules.Automation.AutomationMessageDispatcher.ClaimAndDispatchAsync(System.Guid, System.Threading.CancellationToken)`
  Type: `CanDoItAll.Modules.Automation.AutomationMessageDispatcher`
  Kind: `Method`
  Path: src/CanDoItAll.Modules.Automation/AutomationMessagingServices.cs
  Line: 203
- `CanDoItAll.Modules.CrmHr.AiAgentService.SaveAgentProfileAsync(CanDoItAll.Modules.CrmHr.AiAgentProfileEditorModel, System.Threading.CancellationToken)`
  Type: `CanDoItAll.Modules.CrmHr.AiAgentService`
  Kind: `Method`
  Path: src/CanDoItAll.Modules.CrmHr/CrmHrServices.cs
  Line: 3995
- `CanDoItAll.Modules.Workbench.ProjectCrossModuleMutationCoordinator.Begin(System.Guid, string, CanDoItAll.Modules.Workbench.ProjectCrossModuleMutationKind, string, CanDoItAll.Modules.Workbench.ProjectCrossModuleMutationApprovalState)`
  Type: `CanDoItAll.Modules.Workbench.ProjectCrossModuleMutationCoordinator`
  Kind: `Method`
  Path: src/CanDoItAll.Modules.Workbench/ProjectWorkbenchCrossModuleMutations.cs
  Line: 75

## Usage Summary

- Total callers: 111
- Total clusters: 16
- Omitted callers: 33
- Cluster: `CanDoItAll.Modules.Workbench` / `CanDoItAll.Modules.Workbench`
  Caller count: 38
  Sample: `CanDoItAll.Modules.Workbench.ProjectCrossModuleMutationCoordinator` -> `CanDoItAll.Modules.Workbench.ProjectCrossModuleMutationCoordinator.Begin(System.Guid, string, CanDoItAll.Modules.Workbench.ProjectCrossModuleMutationKind, string, CanDoItAll.Modules.Workbench.ProjectCrossModuleMutationApprovalState)`
  Path: src/CanDoItAll.Modules.Workbench/ProjectWorkbenchCrossModuleMutations.cs
  Line: 75
  Reason: Invocation usage sample.
- Cluster: `CanDoItAll.Modules.CrmHr` / `CanDoItAll.Modules.CrmHr`
  Caller count: 15
  Sample: `CanDoItAll.Modules.CrmHr.AiAgentService` -> `CanDoItAll.Modules.CrmHr.AiAgentService.SaveAgentProfileAsync(CanDoItAll.Modules.CrmHr.AiAgentProfileEditorModel, System.Threading.CancellationToken)`
  Path: src/CanDoItAll.Modules.CrmHr/CrmHrServices.cs
  Line: 3995
  Reason: Invocation usage sample.
- Cluster: `CanDoItAll.Modules.Automation` / `CanDoItAll.Modules.Automation`
  Caller count: 13
  Sample: `CanDoItAll.Modules.Automation.AutomationMessageDispatcher` -> `CanDoItAll.Modules.Automation.AutomationMessageDispatcher.ClaimAndDispatchAsync(System.Guid, System.Threading.CancellationToken)`
  Path: src/CanDoItAll.Modules.Automation/AutomationMessagingServices.cs
  Line: 203
  Reason: Invocation usage sample.
- Cluster: `CanDoItAll.Modules.Workspace` / `CanDoItAll.Modules.Workspace`
  Caller count: 12
  Sample: `CanDoItAll.Modules.Workspace.ConnectorCommandProcessor` -> `CanDoItAll.Modules.Workspace.ConnectorCommandProcessor.ProcessAsync(System.Guid, string?, System.Threading.CancellationToken)`
  Path: src/CanDoItAll.Modules.Workspace/ConnectorOutboxService.cs
  Line: 14
  Reason: Invocation usage sample.

## File Excerpts

### src/CanDoItAll.Modules.CrmHr/CrmHrServices.cs

- Total lines: 4969
- Selected lines: 141
- Types: None

#### CanDoItAll.Modules.CrmHr.AiAgentService.SaveAgentProfileAsync(CanDoItAll.Modules.CrmHr.AiAgentProfileEditorModel, System.Threading.CancellationToken)

- Kind: `Method`
- Lines: 3994-4121

```csharp

    public async Task<Result<Guid>> SaveAgentProfileAsync(AiAgentProfileEditorModel model, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(model);

        if (model.PartyId == Guid.Empty)
        {
            return Result<Guid>.Failure(Error.Validation("Choose an AI agent before saving the profile.", "crmhr.ai-agent.party-required"));
        }

        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        var party = await dbContext.Set<Party>()
            .SingleOrDefaultAsync(item => item.Id == model.PartyId, cancellationToken);
        if (party is null)
        {
            return Result<Guid>.Failure(Error.Failure("The selected AI agent party could not be found.", "crmhr.ai-agent.party-not-found"));
        }

        if (party.PartyType != PartyType.AiAgent)
        {
            return Result<Guid>.Failure(Error.Validation("Only AI agent parties can carry AI agent operational profiles.", "crmhr.ai-agent.party-type-invalid"));
        }

        if (model.OwnerPartyId == model.PartyId)
        {
            return Result<Guid>.Failure(Error.Validation("An AI agent cannot own itself.", "crmhr.ai-agent.self-owner"));
        }

        ProviderProfile? provider = null;
        if (model.ProviderProfileId is Guid providerProfileId)
        {
            provider = await dbContext.Set<ProviderProfile>()
                .SingleOrDefaultAsync(item => item.Id == providerProfileId, cancellationToken);
            if (provider is null)
            {
                return Result<Guid>.Failure(Error.Validation("Provider profile must reference an existing workspace provider.", "crmhr.ai-agent.provider-invalid"));
            }
        }

        Party? owner = null;
        if (model.OwnerPartyId is Guid ownerPartyId)
        {
            owner = await dbContext.Set<Party>()
                .SingleOrDefaultAsync(item => item.Id == ownerPartyId, cancellationToken);
            if (owner is null || owner.PartyType != PartyType.Person)
            {
                return Result<Guid>.Failure(Error.Validation("Owner must reference an existing person.", "crmhr.ai-agent.owner-invalid"));
            }
        }

        var normalizedExtendedData = NormalizeJson(model.ExtendedDataJson, "{}");
        if (normalizedExtendedData is null)
        {
            return Result<Guid>.Failure(Error.Validation("Extended data must be valid JSON.", "crmhr.ai-agent.extended-data-invalid"));
        }

        var profile = await dbContext.Set<AiAgentProfile>()
            .SingleOrDefaultAsync(item => item.PartyId == model.PartyId, cancellationToken);
        if (profile is null)
        {
            profile = new AiAgentProfile
            {
                PartyId = model.PartyId
            };
            dbContext.Set<AiAgentProfile>().Add(profile);
        }

        profile.ProviderProfileId = model.ProviderProfileId;
        profile.DefaultModel = ResolveDefaultModel(model.DefaultModel, provider?.DefaultModel);
        profile.ExecutionMode = model.ExecutionMode;
        profile.OwnerPartyId = model.OwnerPartyId;
        profile.CapabilityJson = SerializeCapabilities(model.Capabilities);
        profile.ValidationStatus = model.ValidationStatus;
        profile.LastReviewedAtUtc = ToUtcDate(model.LastReviewedOn);
        profile.Notes = model.Notes.Trim();
        profile.ExtendedDataJson = normalizedExtendedData;

        if (owner is not null)
        {
            var ownerRoles = await dbContext.Set<PartyRoleAssignment>()
                .Where(item => item.PartyId == owner.Id)
                .ToListAsync(cancellationToken);
            if (!ownerRoles.Any(item => item.RoleKind == PartyRoleKind.AiSteward))
            {
                dbContext.Set<PartyRoleAssignment>().Add(new PartyRoleAssignment
                {
                    PartyId = owner.Id,
                    RoleKind = PartyRoleKind.AiSteward,
                    Title = "AI steward",
                    IsPrimary = ownerRoles.Count == 0
                });
            }
        }

        party.LastChangedBy = string.IsNullOrWhiteSpace(model.LastChangedBy) ? "crm-hr-ui" : model.LastChangedBy.Trim();
        party.UpdatedAtUtc = clock.GetUtcNow();
        CrmHrAuditWriter.AddEntry(
            dbContext,
            nameof(AiAgentProfile),
            party.Id,
            "AiAgentProfileSaved",
            $"Saved AI agent profile for '{party.DisplayName}'.",
            new
            {
                profile.ExecutionMode,
                profile.ValidationStatus,
                profile.ProviderProfileId,
                profile.OwnerPartyId
            },
            party.LastChangedBy,
            party.IsSensitive,
            party.UpdatedAtUtc);
        await dbContext.SaveChangesAsync(cancellationToken);
        await UpsertAiAgentSearchDocumentAsync(party.Id, cancellationToken);
        await activityStream.RecordAsync(
            new ActivityWriteRequest(
                "CRM / HR",
                "AiAgentProfileSaved",
                $"Saved AI agent profile for {party.DisplayName}",
                $"{profile.ExecutionMode} / {profile.ValidationStatus}",
                ArtifactKind: nameof(AiAgentProfile),
                ArtifactId: party.Id,
                Route: $"/crm-hr/agents?partyId={party.Id}",
                Actor: party.LastChangedBy),
            cancellationToken);
        return Result<Guid>.Success(profile.Id);
    }

```

#### CanDoItAll.Modules.CrmHr.AiAgentService.CloneCapability(CanDoItAll.Modules.CrmHr.AiCapabilityEditorModel)

- Kind: `Method`
- Lines: 4159-4171

```csharp

    private static AiCapabilityEditorModel CloneCapability(AiCapabilityEditorModel capability)
    {
        return new AiCapabilityEditorModel
        {
            Name = capability.Name.Trim(),
            Scope = capability.Scope.Trim(),
            ToolAccess = capability.ToolAccess.Trim(),
            Limitations = capability.Limitations.Trim(),
            Notes = capability.Notes.Trim()
        };
    }

```

### src/CanDoItAll.Modules.Automation/AutomationMessagingServices.cs

- Total lines: 647
- Selected lines: 40
- Types: `CanDoItAll.Modules.Automation.AutomationMessageDispatcher`

#### CanDoItAll.Modules.Automation.AutomationMessageDispatcher.ClaimAndDispatchAsync(System.Guid, System.Threading.CancellationToken)

- Kind: `Method`
- Lines: 202-241

```csharp

    private async Task<bool> ClaimAndDispatchAsync(Guid deliveryId, CancellationToken cancellationToken)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        var now = clock.GetUtcNow();
        var lockToken = Guid.NewGuid().ToString("N");
        var leaseCutoff = now.Subtract(options.Value.DeliveryLeaseDuration);
        var claimedCount = dbContext.Database.IsSqlite()
            ? await TryClaimDeliveryForSqliteAsync(
                dbContext,
                deliveryId,
                now,
                leaseCutoff,
                lockToken,
                cancellationToken)
            : await dbContext.Set<AutomationEnvelopeDeliveryRecord>()
                .Where(item => item.Id == deliveryId)
                .Where(item => item.AvailableAtUtc <= now)
                .Where(item =>
                    item.State == AutomationDeliveryState.Pending ||
                    item.State == AutomationDeliveryState.RetryScheduled ||
                    (item.State == AutomationDeliveryState.Running &&
                     item.LockedAtUtc != null &&
                     item.LockedAtUtc <= leaseCutoff))
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(item => item.State, AutomationDeliveryState.Running)
                    .SetProperty(item => item.AttemptCount, item => item.AttemptCount + 1)
                    .SetProperty(item => item.LastAttemptAtUtc, now)
                    .SetProperty(item => item.UpdatedAtUtc, now)
                    .SetProperty(item => item.CompletedAtUtc, (DateTimeOffset?)null)
                    .SetProperty(item => item.LockedAtUtc, now)
                    .SetProperty(item => item.LockToken, lockToken), cancellationToken);
        if (claimedCount == 0)
        {
            return false;
        }

        return await DispatchClaimedAsync(deliveryId, lockToken, cancellationToken);
    }

```

### src/CanDoItAll.Modules.Workbench/ProjectWorkbenchCrossModuleMutations.cs

- Total lines: 129
- Selected lines: 22
- Types: `CanDoItAll.Modules.Workbench.ProjectCrossModuleMutationCoordinator`

#### CanDoItAll.Modules.Workbench.ProjectCrossModuleMutationCoordinator.Begin(System.Guid, string, CanDoItAll.Modules.Workbench.ProjectCrossModuleMutationKind, string, CanDoItAll.Modules.Workbench.ProjectCrossModuleMutationApprovalState)

- Kind: `Method`
- Lines: 74-95

```csharp
{
    internal ProjectCrossModuleMutationRecord Begin(
        Guid projectId,
        string scopeNodeKey,
        ProjectCrossModuleMutationKind mutationKind,
        string payloadJson,
        ProjectCrossModuleMutationApprovalState approvalState = ProjectCrossModuleMutationApprovalState.NotRequired)
    {
        var timestamp = clock.GetUtcNow();
        return new ProjectCrossModuleMutationRecord
        {
            ProjectId = projectId,
            ScopeNodeKey = scopeNodeKey.Trim(),
            MutationKind = mutationKind,
            Status = ProjectCrossModuleMutationStatus.Pending,
            ApprovalState = approvalState,
            PayloadJson = string.IsNullOrWhiteSpace(payloadJson) ? "{}" : payloadJson,
            CreatedAtUtc = timestamp,
            UpdatedAtUtc = timestamp
        };
    }

```

### src/CanDoItAll.SharedKernel/IClock.cs

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

