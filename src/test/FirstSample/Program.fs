// Learn more about F# at http://fsharp.org

open Confun.Core.Types
open Confun.Core.Api

let m:ConfunMap = [
            "SrcPort", Port 10us
            "DestPort", Port 8080us
            "DatabaseConnection", Group [
                "ConnectionString", Str "ms-sql.localhost:9090"
            ]
        ]

[<EntryPoint>]
let main argv =
    let res = MapValidator.validate m
    let validationResult = match res with
                            | ErrorValidation error -> ConfigGenerator.printError error
                            | SuccessValidation validatedResult -> ConfigGenerator.generateAFormat validatedResult
    printf "%s" validationResult
    0 // return an integer exit code
