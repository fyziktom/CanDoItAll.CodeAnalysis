# Refactor hotspots

## Primary hotspots

- `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Facts\Persistence\PersistenceFactCollector.cs`
- `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Facts\Persistence\PersistenceSyntaxExplorer.cs`
- `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Facts\Dependencies\DependencyFactCollector.cs`
- `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Application\Services\CodeAnalyticsApplicationService.Build.cs`
- `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Facts\Symbols\SymbolFactsCollector.cs`
- `C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Web\wwwroot\app.css`

## Refactor intent

- Split canonical collection logic from heuristics.
- Keep relationship extraction helpers small and named by responsibility.
- Keep render selection and query composition out of collectors.
- Keep UI style ownership aligned to concrete pages and shared surfaces.
