# Focused-context lab request

## Raw request

> Use [$candoitall-bundle-workflow](C:\\Users\\lucys\\.codex\\skills\\candoitall-bundle-workflow\\SKILL.md) to prepare, validate, execute, and validate a bundle that adds this feature in our code analysis.
>
> Main Goal:
> - providing agent context that is precisely focused to solving some trouble
> - save context window for agent and speed up development
>
> Why we are doing it:
> - when agent sees some exception it usually loads whole files and analyzes them; that costs unnecessary context
> - programmers navigate through relations between functions, classes, and enums instead of rereading whole files
>
> How we can do it:
> - when agent finds an exception or compile error it can ask for a related tree and context around it
> - it should also support analysis when the agent plans a change and needs affected code without loading whole files
> - it starts from the function where request, exception, or bug is, then recursively maps related classes and functions with stop limits
>
> Notes:
> - output must contain exact file references
> - optionally include names and XML summaries for the file scope
> - user or agent can specify just the name of a function, class, property, or similar symbol
> - tags should influence analysis depth and focus, for example `Db`
> - we need a UI page where I can select solution plus optional project, write input, add tags, run analysis, and review output below as accordions per file with selected code parts and line stats
> - the page should help tune the feature, so the output must make it obvious whether the chosen parts are helpful or noisy

## Normalized scope delta

- Add prompt-text and diagnostic-text entry, not only explicit ids.
- Add tag-guided scoring and pruning.
- Add code excerpts grouped by file with line-count stats.
- Add a dedicated lab page for tuning and feedback.
