namespace Confun.Core.Processing

open System.IO

open Confun.Core.Processing.Api
open Confun.Core.Types

module ConfigGenerator =
    let generate (configMapGenerator: ConfigMapGenerator) (configFile: ValidatedConfigFile) =
        Directory.CreateDirectory configFile.DirectoryPath |> ignore
        let configPath = Path.Combine(configFile.DirectoryPath, configFile.Name)
        try
            File.WriteAllText(configPath, (configMapGenerator configFile.ValidatedParamsMap))
            Ok (sprintf "File '%s' is successfully generated" configPath)
        with e -> Error [ (GenerationError (sprintf "Generation process for '%s' failed with exception: '%A'" configPath e)) ]

    let generateAll (configMapGenerator: ConfigMapGenerator) (configs: ValidatedConfigFile list) =
        configs 
            |> List.map (generate configMapGenerator)
            |> ConfunError.aggregateResults
