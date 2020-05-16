namespace Confun.Core.Processing

open System.IO

open Confun.Core.Processing.Api
open Confun.Core.Types


module ConfigGenerator =
    let printError (ValidationError error) = sprintf "Validation error: '%A'" error

    let printErrors (errors: ValidationError list) =
        sprintf "Validation errors: '%A'" (Seq.toList errors)

    let generateConfig (configMapGenerator: ConfigMapGenerator) (configFile: ValidatedConfigFile) =
        try
            File.WriteAllText
                (Path.Combine(configFile.DirectoryPath, configFile.Name), (configMapGenerator configFile.ValidatedParamsMap))
            Ok "Success"
        with e -> Error("Error: " + e.ToString())
