# Open Source Packaging And Publishing Readiness

## Status

- `Completed`

## Objective

- Prepare repository and project metadata for open-source publishing after implementation boundaries are stable.

## Success Criteria

- License, security, contribution, package metadata, packability, validation, and release commands are explicit and verified.
- Shipping and non-shipping projects are separated.

## Covered Inputs

- `IN-001`
- `IN-002`
- `IN-005`
- `REQ-009`
- `REQ-010`

## Prerequisites

- `SB01` validation baseline passed.
- `SB02` architecture/package boundary decision completed.
- `SB03` through `SB06` completed or blocked with explicit scope if package contents depend on them.

## Exact Source References

- `repo://Directory.Build.props`
- `repo://CanDoItAll.CodeAnalsis.slnx`
- `repo://README.md`
- `repo://src/CanDoItAll.CodeAnalytics.Abstractions/CanDoItAll.CodeAnalytics.Abstractions.csproj`
- `repo://src/CanDoItAll.CodeAnalytics.Domain/CanDoItAll.CodeAnalytics.Domain.csproj`
- `repo://src/CanDoItAll.CodeAnalytics.Application/CanDoItAll.CodeAnalytics.Application.csproj`
- `repo://src/CanDoItAll.CodeAnalytics.Web/CanDoItAll.CodeAnalytics.Web.csproj`
- `repo://tools/ComparisonHarness/README.md`
- `repo://reference/tool-surface-proposal.json`
- `repo://reference/CanDoItAll.Mcp.CodeAnalytics.settings.example.json`

## Deliverables

- License file and package license metadata.
- Security policy and contribution guidance.
- Packability matrix for production libraries, Web sandbox, tools, tests, and future driver.
- Package metadata: IDs, descriptions, repository URL, readme, tags, authors/owners as appropriate.
- Package validation and pack command transcripts.
- Release checklist and non-shipping exclusions.

## Dependency Impact

- `SB08` docs depend on final package metadata, release commands, and shipping matrix.

## Validation Depth

- Process-critical publishing readiness.

## Implementation Steps

1. Reconfirm package/project decisions from `SB02`.
2. Add OSS repository files and package metadata.
3. Decide whether Web sandbox is packable, sample-only, or app-only.
4. Ensure tools/tests/fixtures are non-shipping unless explicitly included.
5. Run build, tests, pack, package validation, solution structure, and file-length guardrails.
6. Update reference artifacts enough for `SB08` to write final docs.

## Scope Exceptions

- Do not publish to a real external registry unless user explicitly approves.
- Do not implement future MCP driver runtime code here.

## Do Not Do

- Do not make claims in package descriptions that `SB03` or `SB04` did not prove.
- Do not include host-specific secrets, local paths, or private repo references.
- Do not rely on only manual inspection for package contents.

## Acceptance Checklist

- OSS files exist and match intended license/security posture.
- Package metadata is present for every packable production project.
- Non-packable projects are explicitly marked.
- `dotnet pack` or agreed equivalent succeeds.
- Package contents are inspected for unintended artifacts.
- Release checklist is documented.

## Proof Required

- Build/test/pack/package-inspection transcripts.
- Source assertions for metadata and non-shipping exclusions.
- Anti-stub audit transcript.
- Updated execution report rows.

## Browser Validation Logging

- N/A unless Web sandbox packaging/startup behavior is changed; if changed, record desktop-large smoke launch.

## Progression Gate

- `SB08` may write final publishing docs only after package validation and shipping matrix are complete.

## Suggested Agent Prompt

```text
Implement SB07 only. Add OSS publishing metadata and package validation after code boundaries are stable, capture proof, and do not publish externally without explicit user approval.
```
