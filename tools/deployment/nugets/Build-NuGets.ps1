<#
.SYNOPSIS
Builds, validates, tests, and packs the CodeAnalytics libraries.

.DESCRIPTION
This is the repository-owned adapter for the CanDoItAll shared NuGet packaging
contract. It restores unless -NoRestore is supplied, builds and tests unless
-NoBuild is supplied, runs repository validators, and packs the eight shipping
libraries. It never publishes packages.

When -OutputDirectory is omitted, each invocation creates a versioned,
timestamped child below artifacts/packages. When it is supplied, packages are
written directly to that exact directory so CI and cross-repository
orchestration can isolate the output. Use -CreateRunDirectory to create the
versioned child below an explicitly supplied output root.

.PARAMETER Configuration
Build configuration. The default is Release.

.PARAMETER OutputDirectory
Absolute or repository-relative package destination.

.PARAMETER NoRestore
Skips restore when the caller guarantees it has already completed.

.PARAMETER NoBuild
Skips the build and test gates when the caller guarantees they have already
completed. Repository validators still run and packing uses --no-build.

.PARAMETER Version
Temporarily overrides CanDoItAllPackageBaseVersion without editing the
repository.

.PARAMETER PrereleaseSuffix
Appends a prerelease suffix, including its leading hyphen, to the base version.

.PARAMETER CreateRunDirectory
Creates a versioned, timestamped child below an explicitly supplied
OutputDirectory.

.EXAMPLE
.\tools\deployment\nugets\Build-NuGets.ps1 -Version '0.1.5'

.EXAMPLE
.\tools\deployment\nugets\Build-NuGets.ps1 -Version '0.2.0' -PrereleaseSuffix '-preview.1'

.EXAMPLE
.\tools\deployment\nugets\Build-NuGets.ps1 -OutputDirectory C:\packages\codeanalytics
#>
[CmdletBinding(SupportsShouldProcess, ConfirmImpact = 'Medium')]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',

    [string]$OutputDirectory,

    [switch]$NoRestore,

    [switch]$NoBuild,

    [string]$Version = '',

    [string]$PrereleaseSuffix = '',

    [switch]$CreateRunDirectory
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$repositoryRoot = [System.IO.Path]::GetFullPath(
    (Join-Path $PSScriptRoot '..\..\..')
)
$solutionPath = Join-Path $repositoryRoot 'CanDoItAll.CodeAnalsis.slnx'
$directoryBuildPropsPath = Join-Path $repositoryRoot 'Directory.Build.props'
$nugetConfigPath = Join-Path $repositoryRoot 'NuGet.config'

if (-not (Test-Path -LiteralPath $solutionPath -PathType Leaf)) {
    throw "Canonical solution not found at '$solutionPath'."
}

if (-not (Test-Path -LiteralPath $directoryBuildPropsPath -PathType Leaf)) {
    throw "Directory.Build.props was not found at '$directoryBuildPropsPath'."
}

if (-not (Test-Path -LiteralPath $nugetConfigPath -PathType Leaf)) {
    throw "NuGet.config was not found at '$nugetConfigPath'."
}

[xml]$directoryBuildProps = Get-Content -LiteralPath $directoryBuildPropsPath -Raw
$committedVersionNode = $directoryBuildProps.SelectSingleNode(
    '/Project/PropertyGroup/CanDoItAllPackageBaseVersion'
)
if ($null -eq $committedVersionNode) {
    throw "CanDoItAllPackageBaseVersion must be defined in '$directoryBuildPropsPath'."
}

$committedBaseVersion = $committedVersionNode.InnerText.Trim()
if ([string]::IsNullOrWhiteSpace($committedBaseVersion)) {
    throw "CanDoItAllPackageBaseVersion must not be empty in '$directoryBuildPropsPath'."
}

if (
    -not [string]::IsNullOrWhiteSpace($PrereleaseSuffix) -and
    -not $PrereleaseSuffix.StartsWith('-', [StringComparison]::Ordinal)
) {
    throw "PrereleaseSuffix must start with '-', for example '-preview.1'."
}

$effectiveBaseVersion = if ([string]::IsNullOrWhiteSpace($Version)) {
    $committedBaseVersion
}
else {
    $Version.Trim()
}
if ([string]::IsNullOrWhiteSpace($effectiveBaseVersion)) {
    throw 'The effective package base version must not be empty.'
}

$effectiveVersion = "$effectiveBaseVersion$PrereleaseSuffix"
$versionSource = if ([string]::IsNullOrWhiteSpace($Version)) {
    'Directory.Build.props (CanDoItAllPackageBaseVersion)'
}
else {
    'the -Version command-line override'
}

$outputWasSpecified = -not [string]::IsNullOrWhiteSpace($OutputDirectory)
$outputRoot = if ($outputWasSpecified) {
    if ([System.IO.Path]::IsPathRooted($OutputDirectory)) {
        [System.IO.Path]::GetFullPath($OutputDirectory)
    }
    else {
        [System.IO.Path]::GetFullPath((Join-Path $repositoryRoot $OutputDirectory))
    }
}
else {
    Join-Path $repositoryRoot 'artifacts\packages'
}

if (-not $outputWasSpecified -or $CreateRunDirectory) {
    $runTimestamp = Get-Date -Format 'yyyyMMdd-HHmmssfff'
    $OutputDirectory = Join-Path $outputRoot "${effectiveVersion}_$runTimestamp"
}
else {
    $OutputDirectory = $outputRoot
}
$OutputDirectory = [System.IO.Path]::GetFullPath($OutputDirectory)

$normalizedRepositoryRoot = $repositoryRoot.TrimEnd(
    [System.IO.Path]::DirectorySeparatorChar,
    [System.IO.Path]::AltDirectorySeparatorChar
)
$normalizedOutputDirectory = $OutputDirectory.TrimEnd(
    [System.IO.Path]::DirectorySeparatorChar,
    [System.IO.Path]::AltDirectorySeparatorChar
)
if ($normalizedOutputDirectory -eq $normalizedRepositoryRoot) {
    throw 'The package output directory cannot be the repository root.'
}

$packableProjectPaths = @(
    'src\CanDoItAll.CodeAnalytics.Domain\CanDoItAll.CodeAnalytics.Domain.csproj',
    'src\CanDoItAll.CodeAnalytics.Abstractions\CanDoItAll.CodeAnalytics.Abstractions.csproj',
    'src\CanDoItAll.CodeAnalytics.Workspace\CanDoItAll.CodeAnalytics.Workspace.csproj',
    'src\CanDoItAll.CodeAnalytics.Facts\CanDoItAll.CodeAnalytics.Facts.csproj',
    'src\CanDoItAll.CodeAnalytics.Analysis\CanDoItAll.CodeAnalytics.Analysis.csproj',
    'src\CanDoItAll.CodeAnalytics.Rendering\CanDoItAll.CodeAnalytics.Rendering.csproj',
    'src\CanDoItAll.CodeAnalytics.Storage\CanDoItAll.CodeAnalytics.Storage.csproj',
    'src\CanDoItAll.CodeAnalytics.Application\CanDoItAll.CodeAnalytics.Application.csproj'
)
$packableProjects = @(
    foreach ($relativePath in $packableProjectPaths) {
        $projectPath = Join-Path $repositoryRoot $relativePath
        if (-not (Test-Path -LiteralPath $projectPath -PathType Leaf)) {
            throw "Packable project not found at '$projectPath'."
        }

        [pscustomobject]@{
            RelativePath = $relativePath
            ProjectPath = $projectPath
            PackageId = [System.IO.Path]::GetFileNameWithoutExtension($projectPath)
        }
    }
)
$testProjects = @(
    @{
        Path = 'tests\CanDoItAll.CodeAnalytics.Tests.Architecture\CanDoItAll.CodeAnalytics.Tests.Architecture.csproj'
        Timeout = '60s'
    },
    @{
        Path = 'tests\CanDoItAll.CodeAnalytics.Tests.Unit\CanDoItAll.CodeAnalytics.Tests.Unit.csproj'
        Timeout = '600s'
    },
    @{
        Path = 'tests\CanDoItAll.CodeAnalytics.Tests.Integration\CanDoItAll.CodeAnalytics.Tests.Integration.csproj'
        Timeout = '600s'
    },
    @{
        Path = 'tests\CanDoItAll.CodeAnalytics.Tests.Web\CanDoItAll.CodeAnalytics.Tests.Web.csproj'
        Timeout = '600s'
    }
)

$operationParts = [System.Collections.Generic.List[string]]::new()
if (-not $NoRestore) {
    $operationParts.Add('restore the solution')
}
if (-not $NoBuild) {
    $operationParts.Add('build and test the solution')
}
$operationParts.Add('run repository validators')
$operationParts.Add("pack $($packableProjects.Count) projects")
$operation = $operationParts -join ', '

if (-not $PSCmdlet.ShouldProcess($OutputDirectory, $operation)) {
    [pscustomobject]@{
        Repository = Split-Path $repositoryRoot -Leaf
        Solution = Split-Path $solutionPath -Leaf
        Configuration = $Configuration
        PackageVersion = $effectiveVersion
        OutputDirectory = $OutputDirectory
        PackageCount = $packableProjects.Count
        Status = 'Preview'
    }
    return
}

function Invoke-DotNet {
    param(
        [Parameter(Mandatory)]
        [string[]]$Arguments,

        [Parameter(Mandatory)]
        [string]$FailureMessage
    )

    & dotnet @Arguments
    if ($LASTEXITCODE -ne 0) {
        throw "$FailureMessage Exit code: $LASTEXITCODE."
    }
}

$msbuildProperties = @()
if (-not [string]::IsNullOrWhiteSpace($Version)) {
    $msbuildProperties += "-p:CanDoItAllPackageBaseVersion=$effectiveBaseVersion"
}
if (-not [string]::IsNullOrWhiteSpace($PrereleaseSuffix)) {
    $msbuildProperties += "-p:CanDoItAllPackageProofSuffix=$PrereleaseSuffix"
}

Write-Host "Package version: $effectiveVersion"
Write-Host "Version source: $versionSource"
Write-Host "Package output: $OutputDirectory"

if (-not $NoRestore) {
    Write-Host ''
    Write-Host 'Restoring solution...'
    Invoke-DotNet `
        -Arguments (
            @('restore', $solutionPath, '--configfile', $nugetConfigPath) +
            $msbuildProperties
        ) `
        -FailureMessage 'dotnet restore failed.'
}

if (-not $NoBuild) {
    Write-Host ''
    Write-Host 'Building solution...'
    Invoke-DotNet `
        -Arguments (
            @(
                'build',
                $solutionPath,
                '--configuration',
                $Configuration,
                '--no-restore',
                '-warnaserror'
            ) + $msbuildProperties
        ) `
        -FailureMessage 'dotnet build failed.'

    foreach ($testProject in $testProjects) {
        $testProjectPath = Join-Path $repositoryRoot $testProject.Path
        Write-Host ''
        Write-Host "Testing $($testProject.Path)..."
        Invoke-DotNet `
            -Arguments (
                @(
                    'test',
                    $testProjectPath,
                    '--configuration',
                    $Configuration,
                    '--no-build',
                    '--no-restore',
                    '--blame-hang',
                    '--blame-hang-timeout',
                    $testProject.Timeout,
                    '--logger',
                    'console;verbosity=normal'
                ) + $msbuildProperties
            ) `
            -FailureMessage "dotnet test failed for '$($testProject.Path)'."
    }
}

Write-Host ''
Write-Host 'Running repository validators...'
& (Join-Path $repositoryRoot 'eng\Validate-FileLengths.ps1')
& (Join-Path $repositoryRoot 'eng\Validate-SolutionStructure.ps1')

New-Item -ItemType Directory -Path $OutputDirectory -Force | Out-Null

foreach ($project in $packableProjects) {
    Write-Host ''
    Write-Host "Packing $($project.PackageId)..."
    Invoke-DotNet `
        -Arguments (
            @(
                'pack',
                $project.ProjectPath,
                '--configuration',
                $Configuration,
                '--no-build',
                '--no-restore',
                '--output',
                $OutputDirectory,
                '-p:ContinuousIntegrationBuild=true'
            ) + $msbuildProperties
        ) `
        -FailureMessage "dotnet pack failed for '$($project.RelativePath)'."
}

$packagePaths = @(
    foreach ($project in $packableProjects) {
        $packagePath = Join-Path $OutputDirectory (
            "$($project.PackageId).$effectiveVersion.nupkg"
        )
        if (-not (Test-Path -LiteralPath $packagePath -PathType Leaf)) {
            throw "Expected package was not produced: '$packagePath'."
        }
        $packagePath
    }
)
$symbolPackagePaths = @(
    Get-ChildItem -LiteralPath $OutputDirectory `
        -Filter "*.$effectiveVersion.snupkg" `
        -File `
        -ErrorAction SilentlyContinue |
        Sort-Object FullName |
        Select-Object -ExpandProperty FullName
)

Write-Host ''
Write-Host "Packed $($packagePaths.Count) libraries."

[pscustomobject]@{
    Repository = Split-Path $repositoryRoot -Leaf
    Solution = Split-Path $solutionPath -Leaf
    Configuration = $Configuration
    PackageVersion = $effectiveVersion
    OutputDirectory = $OutputDirectory
    Packages = $packagePaths
    SymbolPackages = $symbolPackagePaths
    Status = 'Succeeded'
}
