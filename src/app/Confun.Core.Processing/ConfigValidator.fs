namespace Confun.Core.Processing

open Confun.Core.Processing.Api
open Confun.Core.Types

module ConfigValidator =
    let validate (config: ConfigFile) =
        let validationResult = MapValidator.validate config.ParamsMap
        match validationResult with
        | Error validationErrors -> Error (ConfunError.addPrefixToErrors (sprintf "Error in config: '%s'" config.Name) validationErrors)
        | Ok validatedConfigMap ->
            Ok { Name = config.Name; DirectoryPath = config.DirectoryPath; ValidatedParamsMap = validatedConfigMap }

    let validateAll (configs: ConfigFile list) =
        let validationResults = configs |> List.map validate
        let validationSuccess = validationResults |> List.forall (function | Ok _ -> true | _ -> false)
        if validationSuccess then
            Ok (validationResults  
                    |> List.choose (function 
                                    | Ok result -> Some result
                                    | _ -> None))
        else
            Error (validationResults 
                    |> List.choose (function 
                                    | Error errors -> Some errors
                                    | _ -> None)
                    |> List.concat)

