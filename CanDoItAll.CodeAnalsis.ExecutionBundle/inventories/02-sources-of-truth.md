# Sources of truth

| Area | Canonical owner | Must not drift into |
| --- | --- | --- |
| Workspace inventory | `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Workspace` | UI and renderers |
| Symbol and member facts | `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Facts\Symbols` | UI and application composition |
| Dependency and type relationships | `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Facts\Dependencies` | Mermaid renderers |
| Persistence and entity relationships | `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Facts\Persistence` | UI formatting logic |
| Member context graph | `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Facts` | Web-only state |
| Query orchestration | `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Application` | Domain contracts |
| Export formatting | `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Rendering` | Fact collectors |
