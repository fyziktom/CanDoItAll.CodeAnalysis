# Repository Standards Adoption

This repository follows the reviewed conventions in `CanDoItAll.SharedInfo`. The
repository owns the implementation and the compatibility exceptions below.

## Adopted Baseline

- The root contains repository entry points and policy files.
- Production projects live under `src`, tests under `tests`, and new automation under
  `tools/<area>`.
- NuGet restore sources are deterministic through the root `NuGet.config`.
- Packable projects embed the repository `LICENSE` and use distinct project and source
  repository URLs.
- The desktop sandbox consumes `CanDoItAll.Components.BaseLib` from nuget.org.

## Compatibility Exceptions

- `CanDoItAll.CodeAnalsis.slnx` retains the historical `CodeAnalsis` typo because tests,
  local configuration, and external callers use it as a compatibility entry point. New
  project and namespace names use `CanDoItAll.CodeAnalytics`.
- Durable architecture and product reference material remains under `architecture/` and
  `reference/`. Moving those paths would invalidate links in the completed publishing
  bundle, so a future dedicated documentation migration must update links and evidence
  atomically.
- Existing validation and packing compatibility entry points under `eng/` remain callable.
  New cross-repository-compatible packaging starts at
  `tools/deployment/nugets/Build-NuGets.ps1`.
