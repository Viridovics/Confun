namespace Confun.UnitTests.Generators

open System.Collections.Generic

open Xunit
open FsUnit

open Newtonsoft.Json
open Newtonsoft.Json.Linq

open Confun.Core.Types
open Confun.Generator.Json


module NewtonsoftJsonGeneratorTests =
    [<Fact>]
    let ``Generate valid config to json by NewtonsoftJsonGenerator``() =
        let validatedConfunMap = ValidatedConfunMap [
            "Port", Port 10us
            "String", Str "qwerty"
            "DatabaseConnection", Group [
                "Port", Port 10us
                "String", Str "qwerty"
            ]
            "SystemPorts", Array [| Port 40us; Port 80us; Port 8080us |]
        ]

        let configDictionary = validatedConfunMap 
                                    |> JsonGenerator.generator
                                    |> JsonConvert.DeserializeObject<Dictionary<string, obj>>

        configDictionary.Item("Port") |> should equal 10
        configDictionary.Item("String") |> should equal "qwerty"
        (configDictionary.Item("SystemPorts") :?> JArray).ToObject<int array>() |> should equivalent [| 40; 80; 8080 |]

        let group = (configDictionary.Item("DatabaseConnection") :?> JObject).ToObject<Dictionary<string, obj>>()
        group.Item("Port") |> should equal 10
        group.Item("String") |> should equal "qwerty"


