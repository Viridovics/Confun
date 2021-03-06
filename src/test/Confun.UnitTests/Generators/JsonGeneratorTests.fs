namespace Confun.UnitTests.Generators

open System.Collections.Generic

open Xunit
open FsUnit

open Newtonsoft.Json
open Newtonsoft.Json.Linq

open Confun.Core.Types
open Confun.Generator.Json

module JsonGeneratorTests =
    [<Fact>]
    let ``Generate valid config to json by NewtonsoftJsonGenerator``() =
        let validatedConfunMap = ValidatedConfunMap [
            "IntValue", Int 100
            "FloatValue", Float 20.0
            "Port", Port 10us
            "String", Str "qwerty"
            "DatabaseConnection", Group [
                "Port", Port 10us
                "String", Str "qwerty"
            ]
            "SystemPorts", Array [| Port 40us; Port 80us; Port 8080us |]
            "NullVal", Null
            "NullStr", NullableString "Is not null"
            "Version", Regex (@"\d+\.\d+\.\d+\.\d+", "123.123.432.123")
            "Node", Node ("NodeName", [
                             "IntValue", Int 100
            ])
        ]

        let configDictionary = validatedConfunMap 
                                    |> JsonGenerator.generator
                                    |> JsonConvert.DeserializeObject<Dictionary<string, obj>>

        configDictionary.Item("IntValue") |> should equal 100
        configDictionary.Item("FloatValue") |> should equal 20.0
        configDictionary.Item("Port") |> should equal 10
        configDictionary.Item("String") |> should equal "qwerty"
        (configDictionary.Item("SystemPorts") :?> JArray).ToObject<int array>() |> should equivalent [| 40; 80; 8080 |]
        configDictionary.Item("NullVal") |> should equal null
        configDictionary.Item("NullStr") |> should equal "Is not null"
        configDictionary.Item("Version") |> should equal "123.123.432.123"

        let group = (configDictionary.Item("DatabaseConnection") :?> JObject).ToObject<Dictionary<string, obj>>()
        group.Item("Port") |> should equal 10
        group.Item("String") |> should equal "qwerty"

        let node = (configDictionary.Item("Node") :?> JObject).ToObject<Dictionary<string, obj>>()
        let innerNode = (node.Item("NodeName") :?> JObject).ToObject<Dictionary<string, obj>>()
        innerNode.Item("IntValue") |> should equal 100
