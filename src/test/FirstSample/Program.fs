// Learn more about F# at http://fsharp.org

open Confun.Core.Types
open Confun.Core.Processing
open Confun.Generator.Json
open Confun.Generator.Yaml
open Confun.Generator.Xml

let m:ConfunMap = [
            "IntValue", Int 100
            "PortArray", Array [|
                Port 90us
                Port 8080us
                Port 80us
            |]
            "SrcPort", Port 10us
            "Str", Port 8080us
            "NullVal", Null
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
                    ConfigGenerator.generateConfig
                        { Name = "1.yaml"; DirectoryPath = "."; ParamsMap = validatedResult }
                        YamlGenerator.generator |> printf "%A"
                    ConfigGenerator.generateConfig
                        { Name = "1.xml"; DirectoryPath = "."; ParamsMap = validatedResult }
                        (XmlGenerator.generator "ConfigRoot") |> printf "%A"
                    0
