# Architecture Summary

- Solution: `Fixture.Shop`
- Snapshot ID: `snap-fixture-001`
- Created UTC: `2026-04-07T12:00:00.0000000+00:00`
- Projects: `2`
- Types: `1`
- Members: `2`
- Services: `1`
- Entities: `2`
- Findings: `1`

## Top Findings

- `LAYERING-001` Application depends on Infrastructure: Fixture.Shop.Application references infrastructure.

## Diagnostics

- `DI0001` Factory registration is only partially interpreted.

## Modules

- `Fixture.Shop.Application.Orders` (1 types)

## Persistence

- `ShopDbContext` -> 2 entities
