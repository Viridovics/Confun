// Learn more about F# at http://fsharp.org

open Confun.Core.Types
open Confun.Core.Processing
open Confun.Generator.Json
open Confun.Generator.Yaml
open Confun.Generator.Xml

let packageNode = Node.createNode2 "PackageReference" "Include" "Version"

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
            "Version", Regex (@"\d+\.\d+\.\d+\.\d+", "123.123.432.123")
            "ItemGroup", Array [|
                packageNode (Str "WebSharper") (Str "4.6.1.381")
                packageNode (Str "WebSharper") (Str "4.6.1.381")
            |]
        ]

[<EntryPoint>]
let main argv =
    let config = { Name = "1.xml"; DirectoryPath = "."; ParamsMap = m }
    let configValidationRes = ConfigValidator.validate config
    match configValidationRes with
    | Error error ->
                    printf "%A" error
    | Ok validatedConfig ->
                    ConfigGenerator.generate (XmlGenerator.generator "ConfigRoot") validatedConfig |> printf "%A"

    let res = MapValidator.validate m
    match res with
    | Error error ->
                    printf "%A" error
                    1
    | Ok validatedResult -> 
                    printf "%s" (JsonGenerator.generator validatedResult)
                    ConfigGenerator.generate
                        JsonGenerator.generator
                        { Name = "1.json"; DirectoryPath = "."; ValidatedParamsMap = validatedResult } |> printf "%A"
                    ConfigGenerator.generate
                        YamlGenerator.generator
                        { Name = "1.yaml"; DirectoryPath = "."; ValidatedParamsMap = validatedResult } |> printf "%A"
                    0
