namespace Confun.UnitTests.Generators

open System.Collections.Generic

open Xunit
open FsUnit

open Newtonsoft.Json

open Confun.Core.Types
open Confun.Generator.NewtonsoftJson
open Newtonsoft.Json.Linq


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
        ]

        let configDictionary = validatedConfunMap 
                                    |> JsonGenerator.generator
                                    |> JsonConvert.DeserializeObject<Dictionary<string, obj>>

        configDictionary.Item("Port") |> should equal 10
        configDictionary.Item("String") |> should equal "qwerty"

        let group = (configDictionary.Item("DatabaseConnection") :?> JObject).ToObject<Dictionary<string, obj>>()
        group.Item("Port") |> should equal 10
        group.Item("String") |> should equal "qwerty"


