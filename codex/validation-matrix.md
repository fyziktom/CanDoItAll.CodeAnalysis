# Validation Matrix

Run these commands from the repository root in Windows PowerShell or PowerShell 7.

## Release Gate

```powershell
.\tools\deployment\nugets\Build-NuGets.ps1 -Configuration Release -OutputDirectory .\artifacts\packages
```

## Optional Full-Solution Test

The full solution command is useful as a convenience smoke on an idle machine, but it is not the release gate because concurrent Roslyn and Web operation tests can starve each other locally.

```powershell
dotnet test .\CanDoItAll.CodeAnalsis.slnx --configuration Release --no-build --blame-hang --blame-hang-timeout 600s --logger "console;verbosity=minimal"
```

The Unit, Integration, and Web projects load fixture solutions through Roslyn and MSBuildWorkspace, so they can be quiet for multiple minutes. The segmented release gate keeps Web operation polling from competing with Unit and Integration Roslyn workloads.

## Current Guardrail Notes

- `Validate-FileLengths.ps1` is compatible with Windows PowerShell and PowerShell 7.
- File-length warnings are review prompts; hard failures block release.
- `Validate-SolutionStructure.ps1` protects the canonical `.slnx` shape, required projects, required repository docs, and catch-all folder exclusions.
- `tools/deployment/nugets/Build-NuGets.ps1` is the canonical restore/build/test/pack
  entry point and intentionally excludes the desktop Web sandbox, tools, tests, and
  fixtures from package output.
