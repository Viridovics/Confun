// Learn more about F# at http://fsharp.org

open System

open Confun.Core.Processing
open Confun.Core.Types
open Confun.Generator.Json


let configMap = [
    "AppPort", Port 8080us
    "DatabaseConnection", Group [
        "Instance", Str "localhost:8080"
        "User", Str "UserName"
        "Pwd", Null // Fill in runtime or deployment
        ]
    ]

let config = {
    Name = "app.config.json"
    DirectoryPath = "./App/Configs"
    ParamsMap = configMap 
}

let (||>>) a b = Result.bind b a

[<EntryPoint>]
let main argv =
    let result = config 
                    |> ConfigValidator.validate
                    ||>> (ConfigGenerator.generate JsonGenerator.generator)
    match result with
    | Ok messages ->
        printfn "%A" messages
        0
    | Error errors ->
        printfn "%A" errors
        1
