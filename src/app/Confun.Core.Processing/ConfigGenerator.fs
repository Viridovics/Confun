namespace Confun.Core.Processing

open System.IO

open Confun.Core.Processing.Api
open Confun.Core.Types

module ConfigGenerator =
    let printError error = sprintf "Validation error: '%A'" error

    let printErrors (errors: ConfunError list) =
        sprintf "Validation errors: '%A'" (Seq.toList errors)

    let generate (configMapGenerator: ConfigMapGenerator) (configFile: ValidatedConfigFile) =
        let configPath = Path.Combine(configFile.DirectoryPath, configFile.Name)
        try
            File.WriteAllText(configPath, (configMapGenerator configFile.ValidatedParamsMap))
            Ok (sprintf "File '%s' is successfully generated" configPath)
        with e -> Error [ (GenerationError (sprintf "Generation process for '%s' failed with exception: '%A'" configPath e)) ]

    let generateAll (configMapGenerator: ConfigMapGenerator) (configs: ValidatedConfigFile list) =
        let generationResults = configs |> List.map (generate configMapGenerator)
        let validationSuccess = generationResults |> List.forall (function | Ok _ -> true | _ -> false)
        if validationSuccess then
            Ok (generationResults  
                    |> List.choose (function 
                                    | Ok result -> Some result
                                    | _ -> None))
        else
            Error (generationResults 
                    |> List.choose (function 
                                    | Error errors -> Some errors
                                    | _ -> None)
                    |> List.concat)
