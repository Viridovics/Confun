// Learn more about F# at http://fsharp.org

open Confun.Core.Types
open Confun.Core.Processing
open Confun.Generator.Json

let m:ConfunMap = [
            "SrcPort", Port 10us
            "Str", Port 8080us
            "DatabaseConnection", Group [
                "ConnectionString", Str "ms-sql.localhost:9090"
                "Str2", Str "ms-sql.localhost:9090"
                "ConnectionString3", Str "ms-sql.localhost:9090"
            ]
            "GroupArray", Array [| 
                Group [
                    "RepeatingName", Port 10us
                    "NoRepeatingName", Port 10us
                ]
                Group [
                    "RepeatingName", Port 10us
                    "NoRepeatingName", Port 10us
                ]
            |]
        ]

[<EntryPoint>]
let main argv =
    let res = MapValidator.validate m
    match res with
    | Error error ->
                    printf "%s" (ConfigGenerator.printErrors error)
                    1
    | Ok validatedResult -> 
                    printf "%s" (JsonGenerator.generator validatedResult)
                    ConfigGenerator.generateConfig
                        { Name = "1.json"; DirectoryPath = "."; ParamsMap = validatedResult }
                        JsonGenerator.generator |> printf "%A"
                    0
