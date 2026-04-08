# SB-02 Members implementations and references

## Status

- Completed

## Objective

- Add the remaining standalone symbol-navigation capabilities: members, implementations, and references.

## Covered Inputs

- tools that we are missing and sharptools has them
- both ways how agent can reach informations

## Exact Source References

- C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Domain\Facts\TypeFact.cs
- C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Domain\Facts\MemberFact.cs
- C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Domain\Facts\TypeRelationshipFact.cs
- C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Domain\Facts\MemberRelationshipFact.cs
- C:\repositories\CanDoItAll.CodeAnalsis\src\CanDoItAll.CodeAnalytics.Domain\Facts\ServiceRegistrationFact.cs

## Prerequisites

- SB-00 is complete and trusted.

## Deliverables

- Dedicated members response
- Dedicated implementations response
- Dedicated references response with contextual snippets
- Unit coverage for interface, helper, and UI targets

## Dependency Impact

- Supplies the real SharpTools-style drilldown value; the UI route would be hollow without these responses.

## Validation Depth

- Unit tests
- Manual output inspection against the host comparison scenarios

## Implementation Steps

1. Add members, implementations, and references contracts.
2. Implement member-list and implementation discovery over the snapshot facts.
3. Implement reference tracing with bounded, ordered contextual snippets.
4. Add unit tests for interface, helper, and UI-oriented cases.

## Do Not Do

- Do not dump every possible relationship without ordering and caps.
- Do not conflate implementation discovery with reference tracing.

## Acceptance Checklist

- Members are listed deterministically for a type.
- Implementations or derived types are discoverable.
- References include source path, line, and context text.
- Unit tests cover at least one helper-like and one UI-like target.

## Proof Required

- Passing unit tests
- Sample outputs for the targeted symbol cases

## Browser Validation Logging

- N/A in this phase

## Progression Gate

- UI work may continue only when the service-level symbol outputs are bounded, deterministic, and useful.

## Suggested Agent Prompt

Add the remaining symbol drilldowns. Expose members, implementations, and references as dedicated responses over the existing snapshot facts, keep the results bounded and explicit, and prove them with unit tests.
