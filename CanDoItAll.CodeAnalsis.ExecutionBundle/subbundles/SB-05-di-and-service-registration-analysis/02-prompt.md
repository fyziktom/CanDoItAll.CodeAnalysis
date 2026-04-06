# Prompt for SB-05 — DI and service registration analysis

You are implementing **SB-05** inside the repository `CanDoItAll.CodeAnalsis`.

## Goal
Extract dependency-injection registrations, lifetimes, abstractions, implementations, and obvious composition-root diagnostics from the analyzed solution.

## Read before coding
- overview/05-analysis-pipeline.md
- overview/16-compatibility-and-shared-parts.md
- subbundles/SB-05-di-and-service-registration-analysis/01-scope.md
- subbundles/SB-05-di-and-service-registration-analysis/03-checklist.md
- subbundles/SB-05-di-and-service-registration-analysis/04-validation.md
- subbundles/SB-05-di-and-service-registration-analysis/05-forbidden-patterns.md
- subbundles/SB-05-di-and-service-registration-analysis/06-required-evidence.md

## Also inspect these current CanDoItAll repo files
- src/CanDoItAll.Composition/CanDoItAll.Composition.csproj
- src/CanDoItAll.Mcp.ProjectStructure/Program.cs
- src/CanDoItAll.Mcp.Components/Program.cs

## Required implementation steps
- Implement syntax/semantic analysis for conventional DI extension calls.
- Normalize service registrations into domain facts with lifetime, abstraction, implementation, factory, and source metadata.
- Add fixture cases for direct implementation registration, interface registration, open generics, and ambiguous factories.
- Explicitly mark confidence or support level when the analyzer cannot fully resolve the pattern.

## Cross-cutting guardrails
- Respect the naming map: repo/solution `CanDoItAll.CodeAnalsis`, project family `CanDoItAll.CodeAnalytics.*`, future driver `CanDoItAll.Mcp.CodeAnalytics`.
- Treat `.slnx` as canonical unless a concrete tool blocker proves otherwise.
- Do not clone `CanDoItAll.Mcp.Core` or any host-repo-only MCP runtime types into the standalone libraries.
- Keep facts, insights, and diagnostics separate.
- Keep comments in English and rare. Avoid XML docs unless a public-contract reason clearly justifies them.
- Keep files small and cohesive. If you temporarily create long files for speed, you must split them before closure.
- Avoid dumping-ground folders such as `Helpers`, `Misc`, or `Stuff`.

## Subbundle-specific stop rules
- Do not continue if DI facts are mixed with inferred roles without clear provenance.
- Do not continue if unsupported DI patterns disappear silently.

## Before you call this subbundle done
- run the listed validation commands,
- update or add the required evidence artifacts,
- verify compatibility with the future `CanDoItAll.Mcp.CodeAnalytics` seam,
- do a local cleanup pass on file size and folder hygiene.
