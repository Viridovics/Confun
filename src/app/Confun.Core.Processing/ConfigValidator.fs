namespace Confun.Core.Processing

open Confun.Core.Processing.Api
open Confun.Core.Types

module ConfigValidator =
    let validate (config: ConfigFile) =
        let validationResult = MapValidator.validate config.ParamsMap
        match validationResult with
        | Error validationErrors -> Error (ValidationError.addPrefixToErrors (sprintf "Error in config: '%s'" config.Name) validationErrors)
        | Ok validatedConfigMap ->
            Ok { Name = config.Name; DirectoryPath = config.DirectoryPath; ValidatedParamsMap = validatedConfigMap }
