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

let buildDir  = "./build/"
let dotnetSdkVersion = "3.1.101"

// Lazily install DotNet SDK in the correct version if not available
let installedSdk =
    lazy DotNet.install (fun cfg ->
        { cfg with
            Version = DotNet.CliVersion.Version dotnetSdkVersion
        })

// Set general properties without arguments
let inline compileWithoutArgs arg = 
    DotNet.Options.lift installedSdk.Value arg

// Default target
Target.create "Build" (fun _ ->
  DotNet.exec compileWithoutArgs "build" "Confun.sln" |> ignore
)

//TODO with working dir
Target.create "Run" (fun _ ->
  DotNet.exec compileWithoutArgs "run" "src\.sln" |> ignore
)

Target.create "Restore" (fun _ ->
    DotNet.restore compileWithoutArgs "Confun.sln"
)

"Restore"
   ==> "Build"
// start build
Target.runOrDefault "Build"