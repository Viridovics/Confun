// Learn more about F# at http://fsharp.org

open Confun.Core.Types
open Confun.Core.Processing
open Confun.Generator.NewtonsoftJson

let m:ConfunMap = [
            "SrcPort", Port 10us
            "Str", Port 8080us
            "DatabaseConnection", Group [
                "ConnectionString", Str "ms-sql.localhost:9090"
                "Str2", Str "ms-sql.localhost:9090"
                "ConnectionString3", Str "ms-sql.localhost:9090"
            ]
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
                    0
