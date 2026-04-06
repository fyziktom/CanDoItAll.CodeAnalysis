# SB-05 — DI and service registration analysis

## Objective
Extract dependency-injection registrations, lifetimes, abstractions, implementations, and obvious composition-root diagnostics from the analyzed solution.

## Milestone / priority / actor
- Milestone: `M2`
- Priority: `P0`
- Primary actor: `Analysis maintainer`

## Depends on
- SB-01
- SB-02
- SB-03

## Read first
- overview/05-analysis-pipeline.md
- overview/16-compatibility-and-shared-parts.md

## Current CanDoItAll reference files to inspect
- src/CanDoItAll.Composition/CanDoItAll.Composition.csproj
- src/CanDoItAll.Mcp.ProjectStructure/Program.cs
- src/CanDoItAll.Mcp.Components/Program.cs

## In scope
- Find `IServiceCollection` registration sites and extract service/implementation/lifetime facts where the pattern is statically recognizable.
- Capture multiple registrations, open generics, factories, and ambiguous registrations with explicit diagnostics.
- Link registrations back to source locations and containing projects/modules.

## Out of scope
- No runtime container reflection as the primary path.
- No attempt to guarantee 100% DI discovery for every custom pattern in v1.

## Compatibility rules specific to this subbundle
- The DI analyzer should later help explain current CanDoItAll composition roots and MCP service wiring.
- Preserve enough metadata that a future MCP driver can answer architecture questions without reopening raw source files.

## Expected deliverables
- DI registration fact collector.
- Rule coverage for common AddTransient/AddScoped/AddSingleton patterns and common overloads.
- Diagnostics for unsupported factories or reflection-heavy registration styles.
