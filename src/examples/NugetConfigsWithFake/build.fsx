#r "paket:
    nuget FSharp.Core 4.7.0
    nuget Fake.IO.FileSystem
    nuget Fake.Core.Target
    nuget Fake.Core.ReleaseNotes
    nuget FAKE.Core.Environment
    nuget Fake.DotNet.Cli
    nuget FAKE.Core.Process
    nuget Fake.DotNet.AssemblyInfoFile
    nuget Fake.Tools.Git
    nuget Fake.DotNet.Paket
    nuget Fake.Api.GitHub"
#load "./.fake/build.fsx/intellisense.fsx"


open Fake.Core
open Fake.DotNet

open Fake.Core.TargetOperators

exception GenerationErrorException of string

let dotnetSdkVersion = "3.1.101"

// Lazily install DotNet SDK in the correct version if not available
let installedSdk = lazy (DotNet.install (fun cfg -> { cfg with Version = DotNet.CliVersion.Version dotnetSdkVersion }))

// Set general properties without arguments
let inline compileWithoutArgs arg = DotNet.Options.lift installedSdk.Value arg

let inline compileWithProject projectPath =
    let projectOption = sprintf "--project %s" projectPath
    DotNet.Options.lift installedSdk.Value >> DotNet.Options.withCustomParams (Some projectOption)

let inline compileWithProjectPathAndFsxWorkingDirectory projectPath =
    let projectOption = sprintf "--project %s" projectPath
    DotNet.Options.lift installedSdk.Value 
            >> DotNet.Options.withWorkingDirectory __SOURCE_DIRECTORY__ 
            >> DotNet.Options.withCustomParams (Some projectOption)

let slnPath = System.IO.Path.Combine(__SOURCE_DIRECTORY__, "NugetConfigsWithFake.sln")
let generatorPath = System.IO.Path.Combine(__SOURCE_DIRECTORY__, @"src\NugetConfigsWithFake.ConfigGenerator\NugetConfigsWithFake.ConfigGenerator.fsproj")

// Default target
Target.create "Build" (fun _ -> DotNet.build compileWithoutArgs slnPath)

Target.create "RunConfigGenerator" (fun _ ->
    let result = DotNet.exec (compileWithProjectPathAndFsxWorkingDirectory generatorPath) "run" ""
    if (result.ExitCode <> 0) then raise (GenerationErrorException(sprintf "%A" result.Messages)))

Target.create "Restore" (fun _ -> DotNet.restore compileWithoutArgs slnPath)

"Restore"
    ==> "RunConfigGenerator"
    ==> "Build"

// start build
Target.runOrDefault "Build"
