namespace Confun.Core.Api

module ConfigGenerator =
    let printError (ValidationError error) =
        sprintf "Validation error: '%A'" error

    let generateAFormat (ValidatedConfunMap configMap) =
        sprintf "Config is: '%A'" configMap