namespace Confun.Core.Processing

open System;

open Confun.Core.Processing.Api
open Confun.Core.Types

module ConfigParamsValidation =
    let internal validateOptionNamesUniquenesInList (configParams: ConfigParam list): ValidationResult<ConfunError list> =
        let duplicatesOptionsNames =
            configParams
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

    let internal validateOptionNamesForEmptiness (configParams: ConfigParam list): ValidationResult<ConfunError list> =
        let optionsWithEmptyName =
            configParams
            |> List.filter (fun (optionName, _) -> String.IsNullOrWhiteSpace optionName )

        let validationErrorList =
            optionsWithEmptyName
            |> Seq.map ( (fun (_, configValue) -> configValue) >> (sprintf "Config value '%A' have empty name") >> ValidationError)
            |> Seq.toList

        if List.isEmpty validationErrorList then Valid else Invalid validationErrorList

    let namesUniquenesInGroupValidationStep: ConfigParamValidationStep =
        function
        | name, Group group ->
            let validationResult = validateOptionNamesUniquenesInList group
            match validationResult with
            | Valid -> Valid
            | Invalid errorList ->
                Invalid(ConfunError.addPrefixToErrors (sprintf "Error in Group: '%s'" name) errorList)
        | _ -> Valid

    let namesEmptinessInGroupValidationStep: ConfigParamValidationStep =
        function
        | name, Group group ->
            let validationResult = validateOptionNamesForEmptiness group
            match validationResult with
            | Valid -> Valid
            | Invalid errorList ->
                Invalid(ConfunError.addPrefixToErrors (sprintf "Error in Group: '%s'" name) errorList)
        | _ -> Valid

    let namesUniquenesInNodeValidationStep: ConfigParamValidationStep =
        function
        | name, Node (_, dict) ->
            let validationResult = validateOptionNamesUniquenesInList dict
            match validationResult with
            | Valid -> Valid
            | Invalid errorList ->
                Invalid(ConfunError.addPrefixToErrors (sprintf "Error in Node: '%s'" name) errorList)
        | _ -> Valid    

    let namesEmptinessInNodeValidationStep: ConfigParamValidationStep =
        function
        | name, Node (_, dict) ->
            let validationResult = validateOptionNamesForEmptiness dict
            match validationResult with
            | Valid -> Valid
            | Invalid errorList ->
                Invalid(ConfunError.addPrefixToErrors (sprintf "Error in Node: '%s'" name) errorList)
        | _ -> Valid

    let nodeNameEmptinessValidationStep: ConfigParamValidationStep =
        function
        | name, Node (innerNodeName, _) ->
            if String.IsNullOrWhiteSpace innerNodeName then
                Invalid([ ValidationError (sprintf "Node '%s' have empty NodeName" name)])
            else
                Valid
        | _ -> Valid

    let nullStringValidationStep: ConfigParamValidationStep =
        function
        | name, Str str ->
            if isNull str then
                Invalid [ ValidationError (sprintf "String '%s' is null. If you want to use null string then use Null or NullableString" name) ]
            else
                Valid
        | _ -> Valid

    let regexValidationStep: ConfigParamValidationStep =
        function
        | paramName, Regex (pattern, text) ->
            let fullPattern = sprintf "^%s$" pattern
            let regexResult = System.Text.RegularExpressions.Regex.Match(text, fullPattern)
            if not regexResult.Success then
                Invalid [ ValidationError (sprintf "Regex param '%s' is invalid. Text '%s' is not matched by regex '%s'" paramName text fullPattern) ]
            else
                Valid
        | _ -> Valid

