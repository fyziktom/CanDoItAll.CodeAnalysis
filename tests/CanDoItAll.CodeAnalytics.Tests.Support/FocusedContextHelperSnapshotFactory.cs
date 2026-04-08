using CanDoItAll.CodeAnalytics.Domain.Exports;
using CanDoItAll.CodeAnalytics.Domain.Facts;
using CanDoItAll.CodeAnalytics.Domain.Insights;
using CanDoItAll.CodeAnalytics.Domain.Snapshot;
using CanDoItAll.CodeAnalytics.Domain.Sources;

namespace CanDoItAll.CodeAnalytics.Tests.Support;

public static class FocusedContextHelperSnapshotFactory {
    public static async Task<ArchitectureSnapshot> CreateHighFanInHelperSnapshotAsync(string workspacePath) {
        var sourceRoot = Path.Combine(workspacePath, "src");
        var contractsPath = Path.Combine(sourceRoot, "Test.Helpers", "IClock.cs");
        var infrastructurePath = Path.Combine(sourceRoot, "Test.Infrastructure.Time", "SystemClock.cs");
        var ordersOrderServicePath = Path.Combine(sourceRoot, "Test.Orders", "OrderService.cs");
        var ordersInvoiceServicePath = Path.Combine(sourceRoot, "Test.Orders", "InvoiceService.cs");
        var remindersReminderServicePath = Path.Combine(sourceRoot, "Test.Reminders", "ReminderService.cs");
        var remindersDigestServicePath = Path.Combine(sourceRoot, "Test.Reminders", "DigestService.cs");
        var dashboardPagePath = Path.Combine(sourceRoot, "Test.Dashboard", "DashboardPage.cs");
        var reportsPath = Path.Combine(sourceRoot, "Test.Reports", "ReportBuilder.cs");
        var automationPath = Path.Combine(sourceRoot, "Test.Automation", "CleanupJob.cs");
        var programPath = Path.Combine(sourceRoot, "Test.App", "Program.cs");

        var documents = new List<DocumentFact>();
        await WriteSourceAsync(
            contractsPath,
            """
            namespace Test.Helpers;

            public interface IClock
            {
                DateTimeOffset GetUtcNow();
            }
            """,
            "proj-contracts",
            workspacePath,
            documents);
        await WriteSourceAsync(
            infrastructurePath,
            """
            using Test.Helpers;

            namespace Test.Infrastructure.Time;

            public sealed class SystemClock : IClock
            {
                public DateTimeOffset GetUtcNow()
                {
                    return DateTimeOffset.UtcNow;
                }
            }
            """,
            "proj-infra",
            workspacePath,
            documents);
        await WriteSourceAsync(
            ordersOrderServicePath,
            CreateConsumerSource("Test.Orders", "OrderService", "PlaceOrder"),
            "proj-orders",
            workspacePath,
            documents);
        await WriteSourceAsync(
            ordersInvoiceServicePath,
            CreateConsumerSource("Test.Orders", "InvoiceService", "BuildInvoice"),
            "proj-orders",
            workspacePath,
            documents);
        await WriteSourceAsync(
            remindersReminderServicePath,
            CreateConsumerSource("Test.Reminders", "ReminderService", "SendReminder"),
            "proj-reminders",
            workspacePath,
            documents);
        await WriteSourceAsync(
            remindersDigestServicePath,
            CreateConsumerSource("Test.Reminders", "DigestService", "ComposeDigest"),
            "proj-reminders",
            workspacePath,
            documents);
        await WriteSourceAsync(
            dashboardPagePath,
            CreateConsumerSource("Test.Dashboard", "DashboardPage", "LoadDashboard"),
            "proj-dashboard",
            workspacePath,
            documents);
        await WriteSourceAsync(
            reportsPath,
            CreateConsumerSource("Test.Reports", "ReportBuilder", "BuildReport"),
            "proj-reports",
            workspacePath,
            documents);
        await WriteSourceAsync(
            automationPath,
            CreateConsumerSource("Test.Automation", "CleanupJob", "RunCleanup"),
            "proj-automation",
            workspacePath,
            documents);
        await WriteSourceAsync(
            programPath,
            """
            using Test.Helpers;
            using Test.Infrastructure.Time;

            namespace Test.App;

            public static class Program
            {
                public static void ConfigureServices(IServiceCollection services)
                {
                    services.AddSingleton<IClock, SystemClock>();
                }
            }
            """,
            "proj-app",
            workspacePath,
            documents);

        var solutionPath = Path.Combine(workspacePath, "FocusedContext.Helpers.slnx");
        await File.WriteAllTextAsync(solutionPath, string.Empty);
        var request = new AnalysisRequest(
            solutionPath,
            [],
            [],
            true,
            true,
            true,
            true,
            true);

        var solution = new SolutionFact("FocusedContext.Helpers", solutionPath, 8, documents.Count);
        var projects = new[]
        {
            new ProjectFact("proj-contracts", "Test.Helpers", "src/Test.Helpers/Test.Helpers.csproj", ["net10.0"], [], [], 1),
            new ProjectFact("proj-infra", "Test.Infrastructure.Time", "src/Test.Infrastructure.Time/Test.Infrastructure.Time.csproj", ["net10.0"], ["proj-contracts"], [], 1),
            new ProjectFact("proj-orders", "Test.Orders", "src/Test.Orders/Test.Orders.csproj", ["net10.0"], ["proj-contracts"], [], 2),
            new ProjectFact("proj-reminders", "Test.Reminders", "src/Test.Reminders/Test.Reminders.csproj", ["net10.0"], ["proj-contracts"], [], 2),
            new ProjectFact("proj-dashboard", "Test.Dashboard", "src/Test.Dashboard/Test.Dashboard.csproj", ["net10.0"], ["proj-contracts"], [], 1),
            new ProjectFact("proj-reports", "Test.Reports", "src/Test.Reports/Test.Reports.csproj", ["net10.0"], ["proj-contracts"], [], 1),
            new ProjectFact("proj-automation", "Test.Automation", "src/Test.Automation/Test.Automation.csproj", ["net10.0"], ["proj-contracts"], [], 1),
            new ProjectFact("proj-app", "Test.App", "src/Test.App/Test.App.csproj", ["net10.0"], ["proj-contracts", "proj-infra"], [], 1),
        };
        var modules = new[]
        {
            new ModuleFact("mod-contracts", "proj-contracts", "Test.Helpers", "Test.Helpers", ["ns-contracts"], ["type-clock"]),
            new ModuleFact("mod-infra", "proj-infra", "Test.Infrastructure.Time", "Test.Infrastructure.Time", ["ns-infra"], ["type-system-clock"]),
            new ModuleFact("mod-orders", "proj-orders", "Test.Orders", "Test.Orders", ["ns-orders"], ["type-order-service", "type-invoice-service"]),
            new ModuleFact("mod-reminders", "proj-reminders", "Test.Reminders", "Test.Reminders", ["ns-reminders"], ["type-reminder-service", "type-digest-service"]),
            new ModuleFact("mod-dashboard", "proj-dashboard", "Test.Dashboard", "Test.Dashboard", ["ns-dashboard"], ["type-dashboard-page"]),
            new ModuleFact("mod-reports", "proj-reports", "Test.Reports", "Test.Reports", ["ns-reports"], ["type-report-builder"]),
            new ModuleFact("mod-automation", "proj-automation", "Test.Automation", "Test.Automation", ["ns-automation"], ["type-cleanup-job"]),
        };
        var namespaces = new[]
        {
            new NamespaceFact("ns-contracts", "proj-contracts", "mod-contracts", "Test.Helpers", ["type-clock"]),
            new NamespaceFact("ns-infra", "proj-infra", "mod-infra", "Test.Infrastructure.Time", ["type-system-clock"]),
            new NamespaceFact("ns-orders", "proj-orders", "mod-orders", "Test.Orders", ["type-order-service", "type-invoice-service"]),
            new NamespaceFact("ns-reminders", "proj-reminders", "mod-reminders", "Test.Reminders", ["type-reminder-service", "type-digest-service"]),
            new NamespaceFact("ns-dashboard", "proj-dashboard", "mod-dashboard", "Test.Dashboard", ["type-dashboard-page"]),
            new NamespaceFact("ns-reports", "proj-reports", "mod-reports", "Test.Reports", ["type-report-builder"]),
            new NamespaceFact("ns-automation", "proj-automation", "mod-automation", "Test.Automation", ["type-cleanup-job"]),
        };
        var types = new[]
        {
            new TypeFact("type-clock", "proj-contracts", "mod-contracts", "ns-contracts", "Test.Helpers.IClock", TypeKind.Interface, null, [], ["member-clock-getutcnow"], "Clock contract.", CreateSourceReference(contractsPath, workspacePath, 3, 5)),
            new TypeFact("type-system-clock", "proj-infra", "mod-infra", "ns-infra", "Test.Infrastructure.Time.SystemClock", TypeKind.Class, null, ["Test.Helpers.IClock"], ["member-systemclock-getutcnow"], "Clock implementation.", CreateSourceReference(infrastructurePath, workspacePath, 5, 10)),
            new TypeFact("type-order-service", "proj-orders", "mod-orders", "ns-orders", "Test.Orders.OrderService", TypeKind.Class, null, [], ["member-order-service-placeorder"], "Places orders.", CreateSourceReference(ordersOrderServicePath, workspacePath, 5, 15)),
            new TypeFact("type-invoice-service", "proj-orders", "mod-orders", "ns-orders", "Test.Orders.InvoiceService", TypeKind.Class, null, [], ["member-invoice-service-buildinvoice"], "Builds invoices.", CreateSourceReference(ordersInvoiceServicePath, workspacePath, 5, 15)),
            new TypeFact("type-reminder-service", "proj-reminders", "mod-reminders", "ns-reminders", "Test.Reminders.ReminderService", TypeKind.Class, null, [], ["member-reminder-service-sendreminder"], "Sends reminders.", CreateSourceReference(remindersReminderServicePath, workspacePath, 5, 15)),
            new TypeFact("type-digest-service", "proj-reminders", "mod-reminders", "ns-reminders", "Test.Reminders.DigestService", TypeKind.Class, null, [], ["member-digest-service-composedigest"], "Composes digests.", CreateSourceReference(remindersDigestServicePath, workspacePath, 5, 15)),
            new TypeFact("type-dashboard-page", "proj-dashboard", "mod-dashboard", "ns-dashboard", "Test.Dashboard.DashboardPage", TypeKind.Class, null, [], ["member-dashboard-page-loaddashboard"], "Loads dashboard state.", CreateSourceReference(dashboardPagePath, workspacePath, 5, 15)),
            new TypeFact("type-report-builder", "proj-reports", "mod-reports", "ns-reports", "Test.Reports.ReportBuilder", TypeKind.Class, null, [], ["member-report-builder-buildreport"], "Builds reports.", CreateSourceReference(reportsPath, workspacePath, 5, 15)),
            new TypeFact("type-cleanup-job", "proj-automation", "mod-automation", "ns-automation", "Test.Automation.CleanupJob", TypeKind.Class, null, [], ["member-cleanup-job-runcleanup"], "Runs cleanup.", CreateSourceReference(automationPath, workspacePath, 5, 15)),
        };
        var members = new[]
        {
            new MemberFact("member-clock-getutcnow", "type-clock", "GetUtcNow", MemberKind.Method, "DateTimeOffset", [], CreateSourceReference(contractsPath, workspacePath, 5, 5)),
            new MemberFact("member-systemclock-getutcnow", "type-system-clock", "GetUtcNow", MemberKind.Method, "DateTimeOffset", [], CreateSourceReference(infrastructurePath, workspacePath, 7, 10)),
            new MemberFact("member-order-service-placeorder", "type-order-service", "PlaceOrder", MemberKind.Method, "DateTimeOffset", [], CreateSourceReference(ordersOrderServicePath, workspacePath, 11, 14)),
            new MemberFact("member-invoice-service-buildinvoice", "type-invoice-service", "BuildInvoice", MemberKind.Method, "DateTimeOffset", [], CreateSourceReference(ordersInvoiceServicePath, workspacePath, 11, 14)),
            new MemberFact("member-reminder-service-sendreminder", "type-reminder-service", "SendReminder", MemberKind.Method, "DateTimeOffset", [], CreateSourceReference(remindersReminderServicePath, workspacePath, 11, 14)),
            new MemberFact("member-digest-service-composedigest", "type-digest-service", "ComposeDigest", MemberKind.Method, "DateTimeOffset", [], CreateSourceReference(remindersDigestServicePath, workspacePath, 11, 14)),
            new MemberFact("member-dashboard-page-loaddashboard", "type-dashboard-page", "LoadDashboard", MemberKind.Method, "DateTimeOffset", [], CreateSourceReference(dashboardPagePath, workspacePath, 11, 14)),
            new MemberFact("member-report-builder-buildreport", "type-report-builder", "BuildReport", MemberKind.Method, "DateTimeOffset", [], CreateSourceReference(reportsPath, workspacePath, 11, 14)),
            new MemberFact("member-cleanup-job-runcleanup", "type-cleanup-job", "RunCleanup", MemberKind.Method, "DateTimeOffset", [], CreateSourceReference(automationPath, workspacePath, 11, 14)),
        };
        var memberRelationships = new[]
        {
            CreateInvocation("rel-order", "member-order-service-placeorder", ordersOrderServicePath, workspacePath),
            CreateInvocation("rel-invoice", "member-invoice-service-buildinvoice", ordersInvoiceServicePath, workspacePath),
            CreateInvocation("rel-reminder", "member-reminder-service-sendreminder", remindersReminderServicePath, workspacePath),
            CreateInvocation("rel-digest", "member-digest-service-composedigest", remindersDigestServicePath, workspacePath),
            CreateInvocation("rel-dashboard", "member-dashboard-page-loaddashboard", dashboardPagePath, workspacePath),
            CreateInvocation("rel-report", "member-report-builder-buildreport", reportsPath, workspacePath),
            CreateInvocation("rel-cleanup", "member-cleanup-job-runcleanup", automationPath, workspacePath),
        };
        var typeRelationships = new[]
        {
            CreateTypeRelationship("typerel-order", "type-order-service", ordersOrderServicePath, workspacePath),
            CreateTypeRelationship("typerel-invoice", "type-invoice-service", ordersInvoiceServicePath, workspacePath),
            CreateTypeRelationship("typerel-reminder", "type-reminder-service", remindersReminderServicePath, workspacePath),
            CreateTypeRelationship("typerel-digest", "type-digest-service", remindersDigestServicePath, workspacePath),
            CreateTypeRelationship("typerel-dashboard", "type-dashboard-page", dashboardPagePath, workspacePath),
            CreateTypeRelationship("typerel-report", "type-report-builder", reportsPath, workspacePath),
            CreateTypeRelationship("typerel-cleanup", "type-cleanup-job", automationPath, workspacePath),
        };
        var services = new[]
        {
            new ServiceRegistrationFact(
                "svc-clock",
                "proj-app",
                "mod-infra",
                ServiceLifetimeKind.Singleton,
                "Test.Helpers.IClock",
                "Test.Infrastructure.Time.SystemClock",
                "AddSingleton",
                false,
                CreateSourceReference(programPath, workspacePath, 9, 9)),
        };
        var facts = new ArchitectureFacts(
            solution,
            projects,
            documents,
            modules,
            namespaces,
            types,
            members,
            memberRelationships,
            typeRelationships,
            services,
            [],
            [],
            [],
            []);
        return new ArchitectureSnapshot(
            "1.1.0",
            "0.1.0",
            "snap-helper-001",
            DateTimeOffset.Parse("2026-04-08T11:30:00Z"),
            request,
            facts,
            ArchitectureInsights.Empty,
            ArchitectureExports.Empty,
            []);
    }

    private static MemberRelationshipFact CreateInvocation(string relationshipId, string fromMemberId, string path, string workspacePath) {
        return new MemberRelationshipFact(
            relationshipId,
            fromMemberId,
            "member-clock-getutcnow",
            MemberRelationshipKind.Invocation,
            1,
            CreateSourceReference(path, workspacePath, 13, 13));
    }

    private static TypeRelationshipFact CreateTypeRelationship(string relationshipId, string fromTypeId, string path, string workspacePath) {
        return new TypeRelationshipFact(
            relationshipId,
            fromTypeId,
            "type-clock",
            TypeRelationshipKind.ConstructorParameter,
            1,
            CreateSourceReference(path, workspacePath, 7, 7));
    }

    private static string CreateConsumerSource(string namespaceName, string typeName, string methodName) {
        return $$"""
            using Test.Helpers;

            namespace {{namespaceName}};

            public sealed class {{typeName}}
            {
                private readonly IClock _clock;

                public {{typeName}}(IClock clock)
                {
                    _clock = clock;
                }

                public DateTimeOffset {{methodName}}()
                {
                    return _clock.GetUtcNow();
                }
            }
            """;
    }

    private static async Task WriteSourceAsync(
        string path,
        string content,
        string projectId,
        string workspacePath,
        ICollection<DocumentFact> documents) {
        var directory = Path.GetDirectoryName(path)!;
        Directory.CreateDirectory(directory);
        await File.WriteAllTextAsync(path, content);
        documents.Add(
            new DocumentFact(
                $"doc-{documents.Count + 1}",
                projectId,
                ToRelativePath(path, workspacePath),
                Path.GetFileName(path),
                CountLines(content)));
    }

    private static SourceReference CreateSourceReference(
        string absolutePath,
        string workspacePath,
        int startLine,
        int endLine) {
        return new SourceReference(ToRelativePath(absolutePath, workspacePath), startLine, 1, endLine, 1);
    }

    private static string ToRelativePath(string absolutePath, string workspacePath) {
        return Path.GetRelativePath(workspacePath, absolutePath).Replace('\\', '/');
    }

    private static int CountLines(string content) {
        return content.Replace("\r\n", "\n", StringComparison.Ordinal).Split('\n').Length;
    }
}
