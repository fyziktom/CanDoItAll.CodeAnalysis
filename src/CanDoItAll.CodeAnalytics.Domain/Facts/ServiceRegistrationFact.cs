using CanDoItAll.CodeAnalytics.Domain.Sources;

namespace CanDoItAll.CodeAnalytics.Domain.Facts;

public sealed record ServiceRegistrationFact(
    string ServiceRegistrationId,
    string ProjectId,
    string ModuleId,
    ServiceLifetimeKind Lifetime,
    string ServiceTypeDisplayName,
    string? ImplementationTypeDisplayName,
    string? RegistrationMethod,
    bool UsesFactory,
    SourceReference Source);
