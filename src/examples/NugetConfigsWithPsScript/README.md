# NugetConfigsWithPsScripts

## How to build

> ...\> run .\build.ps1

## Structure

| Project | Description |
| ------- | ----------- |
| NugetConfigsWithPsScript.ConfigGenerator.fsproj |          |
| NugetConfigsWithPsScript.App.csproj |          |
| NugetConfigsWithPsScript.Lib1.csproj |          |
| NugetConfigsWithPsScript.Lib2.csproj |          |
| NugetConfigsWithPsScript.Lib3.csproj |          |

## NugetConfigsWithPsScript.*.csproj

* NugetConfigsWithPsScript.App.csproj
* NugetConfigsWithPsScript.Lib1.csproj
* NugetConfigsWithPsScript.Lib2.csproj
* NugetConfigsWithPsScript.Lib3.csproj

All this projects contain next import:

        <Import Project="Nuget.*.props" /\>

## NugetConfigsWithPsScript.ConfigGenerator.fsproj

### file