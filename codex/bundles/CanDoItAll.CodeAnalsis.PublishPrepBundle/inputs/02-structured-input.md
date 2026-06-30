# Structured Input

## Objective

- Prepare an implementation-ready open-source publishing bundle for `CanDoItAll.CodeAnalytics`.

## Hard Constraints

- Preparation only; do not implement production or test code in this turn.
- Use the CanDoItAll bundle workflow.
- Use the .NET performance scan skill and EF Core query optimization skill in analysis.
- Produce an `.xlsx` checklist and plan.
- Treat the sandbox UI as desktop-large-screen only.
- Documentation planning must be sequenced after the code/API/publishing improvements it documents.

## Normalized Work Themes

- Release validation and guardrail reliability.
- Architecture boundaries and possible project extraction.
- Large-file and mixed-responsibility refactoring.
- Performance hardening of in-memory snapshot query, Roslyn traversal, storage, rendering, and export paths.
- EF/persistence analyzer hardening and optional EF-specific addon isolation.
- Desktop sandbox UI decomposition and large-screen proof.
- Open-source packaging metadata, licensing, publishing, and security posture.
- Documentation overhaul after shipped changes.

## Current Preparation Decisions

- Bundle profile: `initiative`.
- Critical foundations: `SB01`, `SB02`, `SB03`, `SB04`, and `SB08`.
- UI proof scope: large desktop viewport only, with no dedicated small/medium responsive polish.
- EF query conclusion: production `src/` does not execute EF Core queries; EF packages exist in fixtures, while production code statically analyzes EF constructs.
