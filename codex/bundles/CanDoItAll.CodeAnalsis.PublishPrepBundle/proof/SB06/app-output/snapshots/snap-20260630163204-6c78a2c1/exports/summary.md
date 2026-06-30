# Architecture Summary

- Solution: `Fixture.Shop`
- Snapshot ID: `snap-20260630163204-6c78a2c1`
- Created UTC: `2026-06-30T16:32:04.6673940+00:00`
- Projects: `1`
- Types: `3`
- Members: `11`
- Services: `0`
- Entities: `0`
- Findings: `1`

## Top Findings

- `DEPENDENCY-001` Module cycle detected: The module graph contains a strongly connected component.

## Diagnostics

- `WS0002` Msbuild failed when processing the file 'C:\repositories\CanDoItAll.CodeAnalsis\tests\fixtures\Fixture.Shop\src\Fixture.Shop.Application\Fixture.Shop.Application.csproj' with message: Package 'SQLitePCLRaw.lib.e_sqlite3' 2.1.11 has a known high severity vulnerability, https://github.com/advisories/GHSA-2m69-gcr7-jv3q
- `WS0002` Msbuild failed when processing the file 'C:\repositories\CanDoItAll.CodeAnalsis\tests\fixtures\Fixture.Shop\src\Fixture.Shop.Infrastructure\Fixture.Shop.Infrastructure.csproj' with message: Package 'SQLitePCLRaw.lib.e_sqlite3' 2.1.11 has a known high severity vulnerability, https://github.com/advisories/GHSA-2m69-gcr7-jv3q
- `WS0002` Msbuild failed when processing the file 'C:\repositories\CanDoItAll.CodeAnalsis\tests\fixtures\Fixture.Shop\src\Fixture.Shop.Web\Fixture.Shop.Web.csproj' with message: Package 'SQLitePCLRaw.lib.e_sqlite3' 2.1.11 has a known high severity vulnerability, https://github.com/advisories/GHSA-2m69-gcr7-jv3q

## Modules

- `Fixture.Shop.Application.Notifications` (1 types)
- `Fixture.Shop.Application.Orders` (2 types)

## Persistence
