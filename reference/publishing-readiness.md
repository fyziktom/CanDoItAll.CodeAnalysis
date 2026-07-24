# Publishing Readiness

This repository publishes reusable CodeAnalytics libraries and keeps the desktop sandbox, tests, fixtures, and local harnesses non-shipping.

## Package Matrix

| Project | Packable | Package role |
| --- | --- | --- |
| `src/CanDoItAll.CodeAnalytics.Domain` | Yes | Immutable snapshot, fact, diagnostic, source, and export models. |
| `src/CanDoItAll.CodeAnalytics.Abstractions` | Yes | Public commands, queries, responses, enums, and service contract. |
| `src/CanDoItAll.CodeAnalytics.Workspace` | Yes | MSBuildWorkspace and Roslyn loading helpers. |
| `src/CanDoItAll.CodeAnalytics.Facts` | Yes | Roslyn facts, DI facts, symbol facts, and static EF Core persistence metadata. |
| `src/CanDoItAll.CodeAnalytics.Analysis` | Yes | Findings and insight derivation over collected facts. |
| `src/CanDoItAll.CodeAnalytics.Rendering` | Yes | Markdown and Mermaid export rendering. |
| `src/CanDoItAll.CodeAnalytics.Storage` | Yes | File-system snapshot, recent index, and export storage driver. |
| `src/CanDoItAll.CodeAnalytics.Application` | Yes | Engine facade over snapshot build and query workflows. |
| `src/CanDoItAll.CodeAnalytics.Web` | No | Desktop-large sandbox app only. |
| `tools/*` | No | Local scenario and comparison harnesses only. |
| `tests/*` and `tests/fixtures/*` | No | Validation and Roslyn fixture inputs only. |

## Package Metadata

All packable projects inherit common metadata from `Directory.Build.props` and
`Directory.Build.targets`: the MIT-derived repository license file, the
`https://aicandoitall.com` project URL, canonical source repository URL, authors, package
tags, version prefix, SourceLink repository metadata, and root README packaging. Each
packable production project declares an explicit description in its `.csproj`.

## Pack Commands

```powershell
.\tools\deployment\nugets\Build-NuGets.ps1 -Configuration Release -OutputDirectory .\artifacts\packages
```

Inspect package contents before publishing:

```powershell
Get-ChildItem .\artifacts\packages -Filter *.nupkg | ForEach-Object {
    tar -tf $_.FullName
}
```

The package set must not contain Web app assets, tests, fixtures, Codex bundle proof, local snapshot output, `.artifacts` contents, or machine-local paths.

## Release Gate

Before publishing, rerun the segmented validation matrix in `codex/validation-matrix.md`, then rerun package contents and nuspec inspection against the final `.nupkg` files.

## Not Published In This Wave

- The future `CanDoItAll.Mcp.CodeAnalytics` host driver.
- Optional split packages such as `Facts.EfCore`, `FocusedContext`, `SymbolQueries`, or `Storage.FileSystem`.
- Desktop sandbox as a NuGet package.
- Scenario harnesses as tools.
