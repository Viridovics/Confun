# NugetConfigsWithPsScripts

## How to build

> ...\> run .\build.ps1

## Example motivation

Attempt to unify nuget package versions between projects. Before building all projects build script generate next configs:

    <Project>
        <ItemGroup>
            <PackageReference Include="NLog" Version="4.7.1" />
            <PackageReference Include="Newtonsoft.Json" Version="12.0.3" />
        </ItemGroup>
    </Project>

## Structure

| Project | Description |
| ------- | ----------- |
| NugetConfigsWithPsScript.ConfigGenerator.fsproj | Project is runned before all other project. Project generate nuget configs for other projects. |
| NugetConfigsWithPsScript.App.csproj | This project refers to NLog nuget package, Lib1, Lib2, Lib3 |
| NugetConfigsWithPsScript.Lib1.csproj | This project refers to Newtonsoft.Json, NLog nuget packages |
| NugetConfigsWithPsScript.Lib2.csproj | This project refers to NLog nuget package |
| NugetConfigsWithPsScript.Lib3.csproj | This project refers to Newtonsoft.Json, NLog nuget packages |

## NugetConfigsWithPsScript.*.csproj

* NugetConfigsWithPsScript.App.csproj
* NugetConfigsWithPsScript.Lib1.csproj
* NugetConfigsWithPsScript.Lib2.csproj
* NugetConfigsWithPsScript.Lib3.csproj

All this projects contain next import:

        <Import Project="Nuget.*.props" /\>

## NugetConfigsWithPsScript.ConfigGenerator.fsproj

### NugetPackages.fs
Nuget packages that used by projects are described here.

### Configs.fs
Conigs for nuget (location, content and other) that used by projects are described here.

### Program.fs
Validating and generating.