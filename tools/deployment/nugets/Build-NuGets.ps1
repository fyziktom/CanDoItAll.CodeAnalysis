[CmdletBinding(SupportsShouldProcess, ConfirmImpact = 'Medium')]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',

    [string]$OutputDirectory,

    [switch]$NoRestore
)

$ErrorActionPreference = 'Stop'

$repositoryRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..\..')).Path
$solutionPath = Join-Path $repositoryRoot 'CanDoItAll.CodeAnalsis.slnx'
$nugetConfigPath = Join-Path $repositoryRoot 'NuGet.config'

if (-not $OutputDirectory) {
    $OutputDirectory = Join-Path $repositoryRoot 'artifacts\packages'
}
elseif (-not [System.IO.Path]::IsPathRooted($OutputDirectory)) {
    $OutputDirectory = Join-Path $repositoryRoot $OutputDirectory
}

$OutputDirectory = [System.IO.Path]::GetFullPath($OutputDirectory)
$packableProjects = @(
    'src\CanDoItAll.CodeAnalytics.Domain\CanDoItAll.CodeAnalytics.Domain.csproj',
    'src\CanDoItAll.CodeAnalytics.Abstractions\CanDoItAll.CodeAnalytics.Abstractions.csproj',
    'src\CanDoItAll.CodeAnalytics.Workspace\CanDoItAll.CodeAnalytics.Workspace.csproj',
    'src\CanDoItAll.CodeAnalytics.Facts\CanDoItAll.CodeAnalytics.Facts.csproj',
    'src\CanDoItAll.CodeAnalytics.Analysis\CanDoItAll.CodeAnalytics.Analysis.csproj',
    'src\CanDoItAll.CodeAnalytics.Rendering\CanDoItAll.CodeAnalytics.Rendering.csproj',
    'src\CanDoItAll.CodeAnalytics.Storage\CanDoItAll.CodeAnalytics.Storage.csproj',
    'src\CanDoItAll.CodeAnalytics.Application\CanDoItAll.CodeAnalytics.Application.csproj'
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

if (-not (Test-Path -LiteralPath $solutionPath -PathType Leaf)) {
    throw "Canonical solution not found at '$solutionPath'."
}

$operation = if ($NoRestore) {
    'Build, test, and pack'
}
else {
    'Restore, build, test, and pack'
}
if (-not $PSCmdlet.ShouldProcess(
        $OutputDirectory,
        "$operation '$solutionPath'"
    )) {
    [pscustomobject]@{
        Repository = Split-Path $repositoryRoot -Leaf
        Solution = Split-Path $solutionPath -Leaf
        Configuration = $Configuration
        OutputDirectory = $OutputDirectory
        PackageCount = $packableProjects.Count
        Status = 'Preview'
    }
    return
}

New-Item -ItemType Directory -Path $OutputDirectory -Force | Out-Null

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

if (-not $NoRestore) {
    Invoke-DotNet `
        -Arguments @('restore', $solutionPath, '--configfile', $nugetConfigPath) `
        -FailureMessage 'dotnet restore failed.'
}

Invoke-DotNet `
    -Arguments @(
        'build',
        $solutionPath,
        '--configuration',
        $Configuration,
        '--no-restore',
        '-warnaserror'
    ) `
    -FailureMessage 'dotnet build failed.'

foreach ($testProject in $testProjects) {
    $testProjectPath = Join-Path $repositoryRoot $testProject.Path
    Invoke-DotNet `
        -Arguments @(
            'test',
            $testProjectPath,
            '--configuration',
            $Configuration,
            '--no-build',
            '--blame-hang',
            '--blame-hang-timeout',
            $testProject.Timeout,
            '--logger',
            'console;verbosity=normal'
        ) `
        -FailureMessage "dotnet test failed for '$($testProject.Path)'."
}

& (Join-Path $repositoryRoot 'eng\Validate-FileLengths.ps1')
if ($LASTEXITCODE -ne 0) {
    throw "File-length validation failed with exit code $LASTEXITCODE."
}

& (Join-Path $repositoryRoot 'eng\Validate-SolutionStructure.ps1')
if ($LASTEXITCODE -ne 0) {
    throw "Solution-structure validation failed with exit code $LASTEXITCODE."
}

foreach ($project in $packableProjects) {
    $projectPath = Join-Path $repositoryRoot $project
    Invoke-DotNet `
        -Arguments @(
            'pack',
            $projectPath,
            '--configuration',
            $Configuration,
            '--no-build',
            '--no-restore',
            '--output',
            $OutputDirectory,
            '-p:ContinuousIntegrationBuild=true'
        ) `
        -FailureMessage "dotnet pack failed for '$project'."
}

[pscustomobject]@{
    Repository = Split-Path $repositoryRoot -Leaf
    Solution = Split-Path $solutionPath -Leaf
    Configuration = $Configuration
    OutputDirectory = $OutputDirectory
    PackageCount = $packableProjects.Count
    Status = 'Succeeded'
}
