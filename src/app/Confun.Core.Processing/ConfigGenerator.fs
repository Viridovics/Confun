namespace Confun.Core.Processing

open Confun.Core.Types

module ConfigGenerator =
    let printError (ValidationError error) =
        sprintf "Validation error: '%A'" error

    let printErrors (errors: ValidationError list) =
        sprintf "Validation errors: '%A'" (Seq.toList errors)

    let generateAFormat (ValidatedConfunMap configMap) =
        sprintf "Config is: '%A'" configMap
