[CmdletBinding()]
param(
    [string]$Configuration = "Debug",
    [string]$OutputPath,
    [switch]$NoBuild
)

$ErrorActionPreference = "Stop"

$repoRoot = Resolve-Path (Join-Path $PSScriptRoot "..")
if ([string]::IsNullOrWhiteSpace($OutputPath)) {
    $OutputPath = Join-Path $repoRoot ".artifacts\packages"
}

if (-not [System.IO.Path]::IsPathRooted($OutputPath)) {
    $OutputPath = Join-Path $repoRoot $OutputPath
}

$packableProjects = @(
    "src\CanDoItAll.CodeAnalytics.Domain\CanDoItAll.CodeAnalytics.Domain.csproj",
    "src\CanDoItAll.CodeAnalytics.Abstractions\CanDoItAll.CodeAnalytics.Abstractions.csproj",
    "src\CanDoItAll.CodeAnalytics.Workspace\CanDoItAll.CodeAnalytics.Workspace.csproj",
    "src\CanDoItAll.CodeAnalytics.Facts\CanDoItAll.CodeAnalytics.Facts.csproj",
    "src\CanDoItAll.CodeAnalytics.Analysis\CanDoItAll.CodeAnalytics.Analysis.csproj",
    "src\CanDoItAll.CodeAnalytics.Rendering\CanDoItAll.CodeAnalytics.Rendering.csproj",
    "src\CanDoItAll.CodeAnalytics.Storage\CanDoItAll.CodeAnalytics.Storage.csproj",
    "src\CanDoItAll.CodeAnalytics.Application\CanDoItAll.CodeAnalytics.Application.csproj"
)

New-Item -ItemType Directory -Force -Path $OutputPath | Out-Null

foreach ($project in $packableProjects) {
    $projectPath = Join-Path $repoRoot $project
    $packArgs = @(
        "pack",
        $projectPath,
        "--configuration",
        $Configuration,
        "--output",
        $OutputPath
    )

    if ($NoBuild) {
        $packArgs += "--no-build"
    }

    & dotnet @packArgs
    if ($LASTEXITCODE -ne 0) {
        throw "dotnet pack failed for $project"
    }
}

Write-Host "Packed $($packableProjects.Count) release projects to $OutputPath."
