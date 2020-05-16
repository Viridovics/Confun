// Learn more about F# at http://fsharp.org

open System

open Confun.Core.Types
open Confun.Core.Processing
open Confun.Generator.Xml

open Configs

[<EntryPoint>]
let main argv =
    //TODO: simplify this
    let configs = [ lib1NugetConfig; lib2NugetConfig; lib3NugetConfig; appNugetConfig ]
    let validationResults = configs |> Seq.map ConfigValidator.validate |> Seq.toList
    let validationSuccess = validationResults |> List.forall (function | Ok _ -> true | _ -> false)
    if validationSuccess then
        let xmlGenerator = XmlGenerator.generator "Project"
        let configGenerator = ConfigGenerator.generateConfig xmlGenerator
        let generationResult = validationResults 
                                    |> List.choose (function 
                                                    | Ok validatedConfig -> Some validatedConfig
                                                    | _ -> None)
                                    |> List.map configGenerator
        if (generationResult |> Seq.forall (function | Ok _ -> true | _ -> false)) then
            0
        else
            printfn "%A" generationResult
            1
    else
        printfn "%A" (validationResults |> List.choose (function 
                                                        | Error errors -> Some errors
                                                        | _ -> None))
        1

