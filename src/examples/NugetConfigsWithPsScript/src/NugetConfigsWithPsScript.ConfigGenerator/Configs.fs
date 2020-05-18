module Configs

open Confun.Core.Types
open NugetPackages

// Remarks
// 1. You can move configs to another files
// 2. You can create helper function for ("ItemGroup", Array ...) in module NugetPackages
let lib1NugetConfig = {
    Name = "Nuget.Lib1.props"
    DirectoryPath = ".\src\NugetConfigsWithPsScript.Lib1"
    ParamsMap = [
        "ItemGroup", Array [|
            NLogPackage
            NewtonSoftJsonPackage
        |]
    ]
}

let lib2NugetConfig = {
    Name = "Nuget.Lib2.props"
    DirectoryPath = ".\src\NugetConfigsWithPsScript.Lib2"
    ParamsMap = [
        "ItemGroup", Array [|
            NLogPackage
        |]
    ]
}

let lib3NugetConfig = {
    Name = "Nuget.Lib3.props"
    DirectoryPath = ".\src\NugetConfigsWithPsScript.Lib3"
    ParamsMap = [
        "ItemGroup", Array [|
            NLogPackage
            NewtonSoftJsonPackage
        |]
    ]
}

let appNugetConfig = {
    Name = "Nuget.App.props"
    DirectoryPath = ".\src\NugetConfigsWithPsScript.App"
    ParamsMap = [
        "ItemGroup", Array [|
            NLogPackage
        |]
    ]
}