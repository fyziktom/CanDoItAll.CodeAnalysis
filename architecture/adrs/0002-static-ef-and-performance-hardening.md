# ADR 0002: Static EF Analyzer And Performance Hardening Scope

## Status

Accepted

## Context

The publishing-prep review required EF Core and performance analysis before open-source release. Production source code does not run EF Core queries against a database; it statically analyzes C# source and symbols. Performance risks were concentrated around dynamic regex matching, source-file reads, export path handling, and large orchestration files.

## Decision

Treat EF Core support as static persistence metadata analysis for this publishing wave. Do not document runtime query optimization capabilities until a future feature adds query-shape analysis with positive and negative tests.

Apply bounded performance and safety hardening where the review found public or hot-path risk:

- Remove dynamic `RegexOptions.Compiled` from user-provided symbol regex searches.
- Add a timeout and fail-closed behavior for regex matching.
- Bound public source reads to workspace-contained files at or below 2 MB.
- Keep export writes inside the snapshot directory.
- Keep readable LINQ/query-shaping code where no measured bottleneck was proven.

## Consequences

- Package and README descriptions may say static EF Core persistence metadata, but must not claim N+1, `AsNoTracking`, split-query, SQL-shape, index, or client-evaluation tuning.
- Future runtime EF query analysis should be implemented as a new analyzer feature or optional fact addon after fact-pack registration is configurable.
- Performance changes should continue to cite scenario or benchmark evidence rather than broad micro-optimization.

## Proof

- `reference/ef-analyzer-capabilities.md`
- `reference/performance-hardening-notes.md`
- `codex/bundles/CanDoItAll.CodeAnalsis.PublishPrepBundle/proof/SB04`
- `codex/bundles/CanDoItAll.CodeAnalsis.PublishPrepBundle/proof/SB05`
