namespace Confun.Generator.Json

open System.Collections.Generic

open Newtonsoft.Json

open Confun.Core.Processing.Api
open Confun.Core.Types

module JsonGenerator =
    let rec private representConfigParamAsSiimpleType: ConfigValue -> obj =
        fun configValue ->
            let toSystemDictionary dictionary =
                let configDictionary = new Dictionary<string, obj>()
                dictionary
                |> List.iter (fun (configName, configValue) ->
                    configDictionary.Add(configName, representConfigParamAsSiimpleType configValue))
                configDictionary

            match configValue with
            | Int i -> i :> obj
            | Float f -> f :> obj
            | Port port -> port :> obj
            | Str str -> str :> obj
            | Array arr ->
                arr
                |> Seq.map representConfigParamAsSiimpleType
                |> Seq.toArray :> obj
            | Group group -> group |> toSystemDictionary :> obj

    let generator: ConfigMapGenerator =
        fun (ValidatedConfunMap confunMap) ->
            let configDictionary = new Dictionary<string, obj>()
            confunMap
            |> List.iter (fun (configName, configValue) ->
                configDictionary.Add(configName, representConfigParamAsSiimpleType configValue))
            JsonConvert.SerializeObject(configDictionary, Formatting.Indented)
