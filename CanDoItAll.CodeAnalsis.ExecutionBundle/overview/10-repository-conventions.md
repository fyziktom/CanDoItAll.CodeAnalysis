# Repository conventions

## Code style alignment with the current CanDoItAll repo

Adopt these conventions because they match the current host repo and improve portability:

- strong typing over stringly-typed logic,
- early returns over deep nesting,
- composition over inheritance,
- explicit boundaries,
- no silent fallback mechanisms,
- clear logging with actionable state,
- full cuddled braces,
- comments rare and in English only.

## XML documentation policy

The host repo guidance says **no XML documentation comments unless explicitly requested**.
For this standalone repo, follow the same default:
- keep XML docs minimal,
- only add them where public contracts clearly benefit,
- do not let documentation noise bloat files.

This does **not** affect the analyzer’s ability to ingest XML docs from target solutions.

## Folder conventions

Prefer purpose-specific folders such as:
- `Configuration/`
- `Contracts/`
- `Collectors/`
- `Analyzers/`
- `Renderers/`
- `Persistence/`
- `Pages/`
- `Components/`
- `Queries/`
- `Services/`

Avoid vague catch-alls such as:
- `Helpers/`
- `Misc/`
- `Stuff/`
- `Util/` (unless truly cohesive and tiny)

## Dependency conventions

- UI depends on Application and its own feature-local concerns only.
- Application orchestrates everything.
- Facts/Workspace stay below Application.
- Rendering/Storage depend on Domain/Abstractions, not UI.
- No transport-specific wrapper types inside reusable libraries.

## Repo-asset conventions

To mirror the host repo later, the standalone repo may include:
- `architecture/adrs/`
- `codex/README.md`
- optional `.codex/agents/` placeholders

Keep these assets thin and obviously portable.
