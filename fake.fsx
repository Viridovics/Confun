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
#load "./.fake/fake.fsx/intellisense.fsx"


open Fake.Core
open Fake.DotNet

open Fake.Core.TargetOperators

exception GenerationErrorException of string

let dotnetSdkVersion = "3.1.101"

// Lazily install DotNet SDK in the correct version if not available
let installedSdk = lazy (DotNet.install (fun cfg -> { cfg with Version = DotNet.CliVersion.Version dotnetSdkVersion }))

// Set general properties without arguments
let inline compileWithProject projectPath =
    let projectOption = sprintf "--project %s" projectPath
    DotNet.Options.lift installedSdk.Value >> DotNet.Options.withCustomParams (Some projectOption)

let inline runTestWithCollectCoverage arg =
    let collectCoverageOption = "/p:CollectCoverage=true /p:CoverletOutputFormat=opencover"
    (DotNet.Options.lift installedSdk.Value >> DotNet.Options.withCustomParams (Some collectCoverageOption)) arg

// Set general properties without arguments
let inline compileWithoutArgs arg = DotNet.Options.lift installedSdk.Value arg

// Default target
Target.create "Build" (fun _ -> DotNet.build compileWithoutArgs "Confun.sln")

//TODO with working dir
Target.create "Run" (fun _ ->
    let result = DotNet.exec (compileWithProject @".\src\test\FirstSample\FirstSample.fsproj") "run" ""
    if (result.ExitCode <> 0) then raise (GenerationErrorException(sprintf "%A" result.Messages)))

Target.create "Restore" (fun _ -> DotNet.restore compileWithoutArgs "Confun.sln")

let artifactsTestsDir  = "./artifacts/tests/"

Target.create "RunTests" (fun _ ->
    DotNet.test runTestWithCollectCoverage @".\src\test\Confun.UnitTests\Confun.UnitTests.fsproj"
)

Target.create "OnlyTests" (fun _ ->
    DotNet.test runTestWithCollectCoverage @".\src\test\Confun.UnitTests\Confun.UnitTests.fsproj"
)

"Restore" 
    ==> "Build"
    ==> "RunTests"
// start build
Target.runOrDefault "RunTests"

// for tests 
// dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
// dotnet tool install --global dotnet-reportgenerator-globaltool --version 4.5.6
// reportgenerator -reports:coverage.opencover.xml -targetdir:"genReport"