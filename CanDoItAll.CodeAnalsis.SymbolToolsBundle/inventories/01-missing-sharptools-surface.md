# Missing SharpTools surface inventory

| SharpTools capability | Current CodeAnalytics status | Planned product answer |
| --- | --- | --- |
| `SearchDefinitions` | Missing | Add dedicated symbol search over types and members with explicit search mode |
| `ViewDefinition` | Missing | Add exact definition response with source excerpt and location metadata |
| `GetMembers` | Partial only | Add dedicated member-list response per type |
| `ListImplementations` | Internal only | Add dedicated implementation and derived-type response |
| `FindReferences` | Internal only | Add dedicated reference tracing response with contextual snippets |

## Non-goals for this pass

- Do not replace focused-context.
- Do not add a second, live Roslyn query pipeline.
- Do not attempt full SharpTools parity for every advanced inspection feature in one pass.
