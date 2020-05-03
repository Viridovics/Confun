namespace Confun.Generator.NewtonsoftJson

open System.Collections.Generic

open Newtonsoft.Json;

open Confun.Core.Processing.Api
open Confun.Core.Types

module JsonGenerator =
    let rec private representConfigOptionAsSiimpleType: (ConfigValue -> obj) = 
        fun configValue ->
            let toSystemDictionary dictionary =
                let configDictionary = new Dictionary<string, obj>()
                dictionary |> List.iter (fun (configName, configValue) -> configDictionary.Add(configName, representConfigOptionAsSiimpleType configValue))
                configDictionary

            match configValue with
            | Port port -> port :> obj
            | Str str -> str :> obj
            | Array arr ->
                arr |> Seq.map representConfigOptionAsSiimpleType |> Seq.toArray :> obj
            | Group group ->
                group |> toSystemDictionary :> obj

    let generator : ConfigGenerator =
        fun (ValidatedConfunMap confunMap) ->
            let configDictionary = new Dictionary<string, obj>()
            confunMap  |> List.iter (fun (configName, configValue) -> configDictionary.Add(configName, representConfigOptionAsSiimpleType configValue))
            JsonConvert.SerializeObject(configDictionary, Formatting.Indented)



