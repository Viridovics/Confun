namespace Confun.UnitTests.Generators

open System.Collections.Generic

open Xunit
open FsUnit

open YamlDotNet.Serialization

open Confun.Core.Types
open Confun.Generator.Yaml

module YamlGeneratorTests =
    [<Fact>]
    let ``Generate valid config to yaml by aaubry/YamlDotNet``() =
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
        let deserializer = (DeserializerBuilder()).Build();
        let configDictionary = validatedConfunMap 
                                    |> YamlGenerator.generator
                                    |> deserializer.Deserialize<Dictionary<string, obj>>

        configDictionary.Item("IntValue") :?> string |> int |> should equal 100
        configDictionary.Item("FloatValue") :?> string |> float |> should equal 20.0
        configDictionary.Item("Port") :?> string |> int |> should equal 10
        configDictionary.Item("String") |> should equal "qwerty"
        configDictionary.Item("SystemPorts") :?> List<obj> |> Seq.map (fun p -> p :?> string |> int) |> should equivalent [| 40; 80; 8080 |]
        configDictionary.Item("NullVal") |> should equal null
        configDictionary.Item("NullStr") |> should equal "Is not null"
        configDictionary.Item("Version") |> should equal "123.123.432.123"

        let group = configDictionary.Item("DatabaseConnection") :?> Dictionary<obj, obj>
        group.Item("Port") :?> string |> int |> should equal 10
        group.Item("String") |> should equal "qwerty"

        let node = configDictionary.Item("Node") :?> Dictionary<obj, obj>
        let innerNode = node.Item("NodeName") :?> Dictionary<obj, obj>
        innerNode.Item("IntValue") :?> string |> int |> should equal 100
