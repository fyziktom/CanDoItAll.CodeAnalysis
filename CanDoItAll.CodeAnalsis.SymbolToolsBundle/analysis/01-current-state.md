# Current state

## Confirmed gap

The current CodeAnalytics product exposes snapshot dashboard, dependencies, services, persistence, types, findings, exports, and focused-context flows, but it still lacks the explicit symbol-navigation capabilities that SharpTools exposes directly.

## Missing capabilities relative to SharpTools

1. `SearchDefinitions` equivalent
   - No explicit symbol-definition search exists.
   - The current types page can filter types and members, but it does not behave like a definition search/start-point tool.
2. `ViewDefinition` equivalent
   - No route or application-service method returns the exact type or member source excerpt as a dedicated response.
3. `GetMembers` equivalent
   - Member listing exists only as an optional expansion on the types page, not as a dedicated symbol drilldown.
4. `ListImplementations` equivalent
   - The product can infer implementations internally for focused-context, but it does not expose them as a standalone query surface.
5. `FindReferences` equivalent
   - The product can infer related callers and type references internally for focused-context, but it does not expose a reference list with contextual snippets as a standalone tool.

## Important implementation reality

- The snapshot already stores:
  - `TypeFact` with source location, base type, and interface names.
  - `MemberFact` with source location and signature details.
  - `TypeRelationshipFact` with relationship kind and source location.
  - `MemberRelationshipFact` with relationship kind and call-site location.
  - `ServiceRegistrationFact` with source location.
- That means a first symbol-navigation layer can be implemented without adding a second analysis pipeline.

## Current risk if left as is

- Focused-context gets stretched into jobs that are better solved by exact symbol tools.
- Shared helpers and high-fan-in interfaces still need a more direct, surgical path than focused-context alone provides.
- The UI currently has no direct way to compare exact definition, implementations, and references on one screen.
