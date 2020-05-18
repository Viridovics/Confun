// Learn more about F# at http://fsharp.org

open System

open Confun.Core.Types
open Confun.Core.Processing
open Confun.Generator.Xml

open Configs

let (||>>) a b = Result.bind b a

[<EntryPoint>]
let main argv =
    let configs = [ lib1NugetConfig; lib2NugetConfig; lib3NugetConfig; appNugetConfig ]
    let xmlGenerator = XmlGenerator.generator "Project"
    let result = configs 
                    |> ConfigValidator.validateAll
                    ||>> ConfigGenerator.generateAll xmlGenerator
    match result with
    | Ok messages ->
        printfn "%A" messages
        0
    | Error errors ->
        printfn "%A" errors
        1