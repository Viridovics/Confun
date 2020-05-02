namespace Confun.Generator.NewtonsoftJson

open System.Collections.Generic

open Newtonsoft.Json;

open Confun.Core.Processing.Api
open Confun.Core.Types

module JsonGenerator =
    let rec private representConfigOptionAsSiimpleType: (ConfigValue -> obj) = function
        | Port port -> port :> obj
        | Str str -> str :> obj
        | Group group ->
            let configDictionary = new Dictionary<string, obj>()
            group |> List.iter (fun (configName, configValue) -> configDictionary.Add(configName, representConfigOptionAsSiimpleType configValue))
            configDictionary :> obj

    let generator : ConfigGenerator =
        fun (ValidatedConfunMap confunMap) ->
            let configDictionary = new Dictionary<string, obj>()
            confunMap  |> List.iter (fun (configName, configValue) -> configDictionary.Add(configName, representConfigOptionAsSiimpleType configValue))
            JsonConvert.SerializeObject(configDictionary, Formatting.Indented)



