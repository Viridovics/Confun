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

let inline runTestWithCollectCoverage arg =
    let collectCoverageOption = "/p:CollectCoverage=true /p:CoverletOutputFormat=opencover"
    (DotNet.Options.lift installedSdk.Value >> DotNet.Options.withCustomParams (Some collectCoverageOption)) arg


let confunSlnPath = System.IO.Path.Combine(__SOURCE_DIRECTORY__, "Confun.sln")
let unitTestsPath = System.IO.Path.Combine(__SOURCE_DIRECTORY__, @"src/test/Confun.UnitTests/Confun.UnitTests.fsproj")
let testAppPath = System.IO.Path.Combine(__SOURCE_DIRECTORY__, @"src/test/Confun.TestApp/Confun.TestApp.fsproj")

// Default target
Target.create "Build" (fun _ -> DotNet.build compileWithoutArgs confunSlnPath)

Target.create "RunTestApp" (fun _ ->
    let result = DotNet.exec (compileWithProjectPathAndFsxWorkingDirectory testAppPath) "run" ""
    if (result.ExitCode <> 0) then raise (GenerationErrorException(sprintf "%A" result.Messages)))

Target.create "Clean" (fun _ -> DotNet.exec compileWithoutArgs "clean" "" |> ignore)

Target.create "Restore" (fun _ -> DotNet.restore compileWithoutArgs confunSlnPath)

Target.create "RunTests" (fun _ ->
    DotNet.test runTestWithCollectCoverage unitTestsPath
)

Target.create "OnlyTests" (fun _ ->
    DotNet.test runTestWithCollectCoverage unitTestsPath
)

"Clean"
    ==> "Restore" 
    ==> "Build"
    ==> "RunTests"
// start build
Target.runOrDefault "RunTests"

// for tests 
// dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
// dotnet tool install --global dotnet-reportgenerator-globaltool --version 4.5.6
// reportgenerator -reports:coverage.opencover.xml -targetdir:"genReport"