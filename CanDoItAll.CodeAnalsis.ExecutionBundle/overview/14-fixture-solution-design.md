# Fixture solution design

## Purpose

The fixture solution exists to prove that the engine can analyze a realistic but controllable C# codebase without depending on the real CanDoItAll repo.

## Required characteristics

- multiple projects with meaningful references,
- interfaces and implementations,
- conventional DI registrations,
- one DbContext with multiple entities and relationships,
- XML documentation on selected types,
- at least one intentional layering issue or cycle,
- at least one unsupported or ambiguous DI/persistence case to trigger diagnostics,
- enough variety to exercise generics, partial types, and module grouping.

## Suggested fixture projects

- `Fixture.Shop.Web`
- `Fixture.Shop.Application`
- `Fixture.Shop.Infrastructure`
- `Fixture.Shop.Contracts`

## Suggested fixture examples

- `IOrderService` / `OrderService`
- `INotificationSender` / `EmailNotificationSender`
- `ShopDbContext`
- `Order`, `OrderLine`, `Customer`
- one circular project or namespace dependency introduced intentionally for rule tests
- one DI factory registration that is intentionally only partially supported

## Guidance

The fixture should be representative, not enormous.
It must stay small enough for fast tests but realistic enough that success on the fixture means something.
