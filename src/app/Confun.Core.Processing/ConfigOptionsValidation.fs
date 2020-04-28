namespace Confun.Core.Processing

open Confun.Core.Processing.Api
open Confun.Core.Types

module ConfigOptionsValidation =
    let internal validateOptionNamesUniquenesInList (configOptions: ConfigOption list): ValidationResult<ValidationError list> =
        let duplicatesOptionsNames =
            configOptions
            |> List.map (function
                | name, _ -> name)
            |> List.groupBy id
            |> List.filter (fun (_, set) -> set.Length > 1)
            |> List.map (fun (key, _) -> key)

        let validationErrorList =
            duplicatesOptionsNames
            |> Seq.map ((sprintf "There is duplicate option name '%s'") >> ValidationError)
            |> Seq.toList

        if List.isEmpty validationErrorList then Valid else Invalid validationErrorList

    let validateNamesUniquenesInGroupOptionStep: ConfigOptionValidationStep =
        function
        | name, Group group ->
            let validationResult = validateOptionNamesUniquenesInList group
            match validationResult with
            | Valid -> Valid
            | Invalid errorList ->
                Invalid(ValidationError.addPrefixToErrors (sprintf "Error in group: '%s'" name) errorList)
        | _ -> Valid
