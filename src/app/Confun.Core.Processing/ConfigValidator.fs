namespace Confun.Core.Processing

open Confun.Core.Types
open Confun.Core.Processing.Api

module ConfigValidator =
    let validateWith extraValidation config =
        let validationResult = MapValidator.validateWith extraValidation config.ParamsMap
        match validationResult with
        | Error validationErrors -> Error (ConfunError.addPrefixToErrors (sprintf "Error in config: '%s'" config.Name) validationErrors)
        | Ok validatedConfigMap ->
            Ok { Name = config.Name; DirectoryPath = config.DirectoryPath; ValidatedParamsMap = validatedConfigMap }

    let validateAllWith extraValidation configs =
        configs 
            |> List.map (validateWith extraValidation)
            |> ConfunError.aggregateResults

    let validate config =
        validateWith ValidationOptions.empty config

    let validateAll configs =
        configs 
            |> List.map validate
            |> ConfunError.aggregateResults

