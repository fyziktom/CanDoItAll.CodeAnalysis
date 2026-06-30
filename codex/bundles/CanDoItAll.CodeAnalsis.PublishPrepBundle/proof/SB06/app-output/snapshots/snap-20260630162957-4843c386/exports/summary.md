# Architecture Summary

- Solution: `Fixture.Shop`
- Snapshot ID: `snap-20260630162957-4843c386`
- Created UTC: `2026-06-30T16:29:57.3139112+00:00`
- Projects: `4`
- Types: `22`
- Members: `67`
- Services: `6`
- Entities: `4`
- Findings: `3`

## Top Findings

- `DEPENDENCY-001` Module cycle detected: The module graph contains a strongly connected component.
- `DEPENDENCY-001` Type cycle detected: The type graph contains a strongly connected component.
- `LAYERING-001` Application depends on Infrastructure: Fixture.Shop.Application directly references an Infrastructure project.

## Diagnostics

- `DI0001` Factory-based registration at src/Fixture.Shop.Web/Program.cs is only partially interpreted.
- `DI0001` Factory-based registration at src/Fixture.Shop.Web/Program.cs is only partially interpreted.
- `EF0003` Persistence pattern HasConversion is only partially interpreted.
- `EF0003` Persistence pattern OwnsOne is only partially interpreted.
- `WS0002` Msbuild failed when processing the file 'C:\repositories\CanDoItAll.CodeAnalsis\tests\fixtures\Fixture.Shop\src\Fixture.Shop.Application\Fixture.Shop.Application.csproj' with message: Package 'SQLitePCLRaw.lib.e_sqlite3' 2.1.11 has a known high severity vulnerability, https://github.com/advisories/GHSA-2m69-gcr7-jv3q
- `WS0002` Msbuild failed when processing the file 'C:\repositories\CanDoItAll.CodeAnalsis\tests\fixtures\Fixture.Shop\src\Fixture.Shop.Infrastructure\Fixture.Shop.Infrastructure.csproj' with message: Package 'SQLitePCLRaw.lib.e_sqlite3' 2.1.11 has a known high severity vulnerability, https://github.com/advisories/GHSA-2m69-gcr7-jv3q
- `WS0002` Msbuild failed when processing the file 'C:\repositories\CanDoItAll.CodeAnalsis\tests\fixtures\Fixture.Shop\src\Fixture.Shop.Web\Fixture.Shop.Web.csproj' with message: Package 'SQLitePCLRaw.lib.e_sqlite3' 2.1.11 has a known high severity vulnerability, https://github.com/advisories/GHSA-2m69-gcr7-jv3q

## Modules

- `Fixture.Shop.Application.Notifications` (1 types)
- `Fixture.Shop.Application.Orders` (2 types)
- `Fixture.Shop.Contracts.Notifications` (1 types)
- `Fixture.Shop.Contracts.Orders` (4 types)
- `Fixture.Shop.Contracts.Persistence` (1 types)
- `Fixture.Shop.Contracts.Shared` (1 types)
- `Fixture.Shop.Infrastructure.Notifications` (1 types)
- `Fixture.Shop.Infrastructure.Persistence` (10 types)
- `Fixture.Shop.Web` (1 types)

## Persistence

- `ReportingDbContext` -> 1 entities
- `ShopDbContext` -> 3 entities
