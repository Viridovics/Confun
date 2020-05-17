namespace Confun.Core.Processing

open Confun.Core.Types

module ConfigValidator =
    let validate (config: ConfigFile) =
        let validationResult = MapValidator.validate config.ParamsMap
        match validationResult with
        | Error validationErrors -> Error (ConfunError.addPrefixToErrors (sprintf "Error in config: '%s'" config.Name) validationErrors)
        | Ok validatedConfigMap ->
            Ok { Name = config.Name; DirectoryPath = config.DirectoryPath; ValidatedParamsMap = validatedConfigMap }

    let validateAll (configs: ConfigFile list) =
        configs 
            |> List.map validate
            |> ConfunError.aggregateResults
