[CmdletBinding()]
param()

$ErrorActionPreference = "Stop"

$repoRoot = Resolve-Path (Join-Path $PSScriptRoot "..")
$solutionPath = Join-Path $repoRoot "CanDoItAll.CodeAnalsis.slnx"
$requiredDirectories = @(
    "src",
    "tests",
    "eng",
    "architecture",
    "architecture\\adrs",
    "codex",
    "reference"
)
$requiredFiles = @(
    ".editorconfig",
    "Directory.Build.props",
    "global.json",
    "LICENSE",
    "SECURITY.md",
    "CONTRIBUTING.md",
    "README.md",
    "codex\\README.md",
    "architecture\\adrs\\README.md",
    "architecture\\adrs\\0001-publishing-boundaries.md",
    "architecture\\adrs\\0002-static-ef-and-performance-hardening.md",
    "architecture\\adrs\\0003-open-source-packaging-and-sandbox-scope.md",
    "eng\\Validate-FileLengths.ps1",
    "eng\\Validate-SolutionStructure.ps1",
    "eng\\Pack-ReleaseProjects.ps1",
    "reference\\compatibility-matrix.md",
    "reference\\publishing-readiness.md",
    "reference\\public-api.md",
    "reference\\desktop-sandbox.md",
    "reference\\reuse-later-vs-do-not-duplicate-now.md",
    "reference\\current-candoitall-mcp-context.md",
    "reference\\current-candoitall-mcp-context.json",
    "reference\\tool-surface-proposal.json",
    "reference\\CanDoItAll.Mcp.CodeAnalytics.settings.example.json",
    "reference\\vscode-mcp-snippet.code-analytics.json",
    "CanDoItAll.CodeAnalsis.slnx"
)
$requiredProjects = @(
    "src/CanDoItAll.CodeAnalytics.Abstractions/CanDoItAll.CodeAnalytics.Abstractions.csproj",
    "src/CanDoItAll.CodeAnalytics.Domain/CanDoItAll.CodeAnalytics.Domain.csproj",
    "src/CanDoItAll.CodeAnalytics.Workspace/CanDoItAll.CodeAnalytics.Workspace.csproj",
    "src/CanDoItAll.CodeAnalytics.Facts/CanDoItAll.CodeAnalytics.Facts.csproj",
    "src/CanDoItAll.CodeAnalytics.Analysis/CanDoItAll.CodeAnalytics.Analysis.csproj",
    "src/CanDoItAll.CodeAnalytics.Rendering/CanDoItAll.CodeAnalytics.Rendering.csproj",
    "src/CanDoItAll.CodeAnalytics.Storage/CanDoItAll.CodeAnalytics.Storage.csproj",
    "src/CanDoItAll.CodeAnalytics.Application/CanDoItAll.CodeAnalytics.Application.csproj",
    "src/CanDoItAll.CodeAnalytics.Web/CanDoItAll.CodeAnalytics.Web.csproj",
    "tests/CanDoItAll.CodeAnalytics.Tests.Support/CanDoItAll.CodeAnalytics.Tests.Support.csproj",
    "tests/CanDoItAll.CodeAnalytics.Tests.Unit/CanDoItAll.CodeAnalytics.Tests.Unit.csproj",
    "tests/CanDoItAll.CodeAnalytics.Tests.Integration/CanDoItAll.CodeAnalytics.Tests.Integration.csproj",
    "tests/CanDoItAll.CodeAnalytics.Tests.Web/CanDoItAll.CodeAnalytics.Tests.Web.csproj",
    "tests/CanDoItAll.CodeAnalytics.Tests.Architecture/CanDoItAll.CodeAnalytics.Tests.Architecture.csproj"
)
$forbiddenFolders = @("Helpers", "Misc", "Stuff", "CommonStuff")

function Get-RepoRelativePath {
    param(
        [Parameter(Mandatory = $true)]
        [string]$RootPath,

        [Parameter(Mandatory = $true)]
        [string]$TargetPath
    )

    $normalizedRoot = (Resolve-Path -LiteralPath $RootPath).Path.TrimEnd([System.IO.Path]::DirectorySeparatorChar, [System.IO.Path]::AltDirectorySeparatorChar)
    $normalizedTarget = (Resolve-Path -LiteralPath $TargetPath).Path

    if ($normalizedTarget.StartsWith($normalizedRoot, [System.StringComparison]::OrdinalIgnoreCase)) {
        return $normalizedTarget.Substring($normalizedRoot.Length).TrimStart([System.IO.Path]::DirectorySeparatorChar, [System.IO.Path]::AltDirectorySeparatorChar)
    }

    return $normalizedTarget
}

foreach ($directory in $requiredDirectories) {
    $path = Join-Path $repoRoot $directory
    if (-not (Test-Path -LiteralPath $path -PathType Container)) {
        throw "Required directory is missing: $directory"
    }
}

foreach ($file in $requiredFiles) {
    $path = Join-Path $repoRoot $file
    if (-not (Test-Path -LiteralPath $path -PathType Leaf)) {
        throw "Required file is missing: $file"
    }
}

if (Test-Path -LiteralPath (Join-Path $repoRoot "CanDoItAll.CodeAnalsis.sln")) {
    throw "Compatibility .sln file exists. SB-00 requires .slnx to remain canonical unless a concrete blocker exists."
}

foreach ($project in $requiredProjects) {
    $path = Join-Path $repoRoot $project
    if (-not (Test-Path -LiteralPath $path -PathType Leaf)) {
        throw "Required project is missing: $project"
    }
}

$solutionXml = [xml](Get-Content -LiteralPath $solutionPath -Raw)
$solutionProjects = $solutionXml.SelectNodes("//Project") |
    ForEach-Object { $_.Attributes["Path"].Value.Replace("\", "/") } |
    Sort-Object
$expectedProjects = $requiredProjects | Sort-Object

if ((Compare-Object -ReferenceObject $expectedProjects -DifferenceObject $solutionProjects).Count -gt 0) {
    $missing = $expectedProjects | Where-Object { $_ -notin $solutionProjects }
    $extra = $solutionProjects | Where-Object { $_ -notin $expectedProjects }

    if ($missing.Count -gt 0) {
        throw "Solution is missing required project entries: $($missing -join ', ')"
    }

    if ($extra.Count -gt 0) {
        throw "Solution contains unexpected project entries: $($extra -join ', ')"
    }
}

$forbiddenMatches = Get-ChildItem -Path (Join-Path $repoRoot "src"), (Join-Path $repoRoot "tests") -Directory -Recurse |
    Where-Object { $forbiddenFolders -contains $_.Name } |
    Sort-Object FullName

if ($forbiddenMatches.Count -gt 0) {
    $paths = $forbiddenMatches | ForEach-Object { Get-RepoRelativePath -RootPath $repoRoot -TargetPath $_.FullName }
    throw "Forbidden catch-all folders were found: $($paths -join ', ')"
}

Write-Host "Solution-structure validation passed."
