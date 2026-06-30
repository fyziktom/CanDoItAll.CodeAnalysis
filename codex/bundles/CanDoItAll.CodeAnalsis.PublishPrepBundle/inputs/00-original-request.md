# Original Request

Raw user request preserved for literal scope closure:

```text
You are senior C# architect you must use [$candoitall-bundle-workflow](C:\\Users\\dell\\.codex\\skills\\candoitall-bundle-workflow\\SKILL.md) to solve this:
IMPORTANT: You are preparing bundle only. do not do implementation yet.

Main goal:
Preparation for publishing of the app

Architect notes:
- we need to do review and hardening and refactoring before publishing as opensource.
- you must identify all messy parts like too long files, mixing responsibilities, too large files not splitted to subcomponents and things like this. use xlsx to do detailed checklists and plan. You must identify proper parts that might be isolated into own projects as drivers, helpers or some addon over engine to improve maintanibility of the code. 
- our sandbox ui is for desktop large screen only. do not waste time on tuning on small and medium screens. 
- I need you to use [$analyzing-dotnet-performance](C:\\Users\\dell\\.codex\\skills\\analyzing-dotnet-performance\\SKILL.md) and [$optimizing-ef-core-queries](C:\\Users\\dell\\.codex\\skills\\optimizing-ef-core-queries\\SKILL.md) to analyze our implementation and fins possible troubles we have.
- analyze all documentation too and what needs to be improved (but this must be done based on the improvements we will do).
```

## Raw Input IDs

| ID | Literal input | Scope note |
| --- | --- | --- |
| `IN-001` | `Preparation for publishing of the app` | Create a preparation bundle only. |
| `IN-002` | `review and hardening and refactoring before publishing as opensource` | Covers architecture, code quality, tests, packaging, docs, and publishing gates. |
| `IN-003` | `identify all messy parts like too long files, mixing responsibilities, too large files not splitted to subcomponents` | Requires repo-grounded hotspot inventory. |
| `IN-004` | `use xlsx to do detailed checklists and plan` | Requires a final `.xlsx` artifact with checklists and plan. |
| `IN-005` | `identify proper parts that might be isolated into own projects as drivers, helpers or some addon over engine` | Requires project extraction candidates and dependency sequencing. |
| `IN-006` | `sandbox ui is for desktop large screen only` | UI proof must use desktop-large viewports; do not spend effort on small/medium responsive tuning. |
| `IN-007` | `use analyzing-dotnet-performance` | Requires performance anti-pattern scan checklist and planned fixes. |
| `IN-008` | `use optimizing-ef-core-queries` | Requires EF Core query/analyzer review, including whether production code actually executes EF queries. |
| `IN-009` | `analyze all documentation too and what needs to be improved (but this must be done based on the improvements we will do)` | Documentation phase must follow structural and API decisions. |
