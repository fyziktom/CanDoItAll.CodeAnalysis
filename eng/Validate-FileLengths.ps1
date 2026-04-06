[CmdletBinding()]
param(
    [int]$ReviewThreshold = 350,
    [int]$MaxLines = 450
)

$ErrorActionPreference = "Stop"

$repoRoot = Resolve-Path (Join-Path $PSScriptRoot "..")
$scanRoots = @(
    (Join-Path $repoRoot "src"),
    (Join-Path $repoRoot "tests"),
    (Join-Path $repoRoot "eng")
)
$extensions = @(".cs", ".razor", ".css", ".ps1")
$violations = [System.Collections.Generic.List[string]]::new()
$warnings = [System.Collections.Generic.List[string]]::new()

$files = foreach ($root in $scanRoots) {
    if (Test-Path -LiteralPath $root) {
        Get-ChildItem -Path $root -Recurse -File |
            Where-Object {
                $_.FullName -notmatch "\\(bin|obj)\\" -and
                $extensions.Contains($_.Extension)
            }
    }
}

foreach ($file in $files | Sort-Object FullName) {
    $lineCount = (Get-Content -LiteralPath $file.FullName).Count
    $relativePath = [System.IO.Path]::GetRelativePath($repoRoot, $file.FullName)

    if ($lineCount -gt $MaxLines) {
        $violations.Add("$relativePath has $lineCount lines and exceeds the hard limit of $MaxLines.")
        continue
    }

    if ($lineCount -gt $ReviewThreshold) {
        $warnings.Add("$relativePath has $lineCount lines and should be reviewed before the next slice.")
    }
}

foreach ($warning in $warnings) {
    Write-Warning $warning
}

if ($violations.Count -gt 0) {
    foreach ($violation in $violations) {
        Write-Error $violation
    }

    throw "File-length validation failed."
}

Write-Host "File-length validation passed."
