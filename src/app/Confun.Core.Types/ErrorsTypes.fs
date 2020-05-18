namespace Confun.Core.Types

open System.Diagnostics.CodeAnalysis

[<ExcludeFromCodeCoverage>]
type ConfunError =
    | ValidationError of string
    | GenerationError of string

module ConfunError =
    let toString =
        function
        | ValidationError error -> error
        | GenerationError error -> error

    let addPrefixToError prefix =
        function
            | ValidationError error ->
                error
                |> (sprintf "%s. %s" prefix)
                |> ValidationError
            | GenerationError error ->
                error
                |> (sprintf "%s. %s" prefix)
                |> GenerationError

    let addPrefixToErrors prefix errorList =
        errorList |> List.map (addPrefixToError prefix)

    let unwrapOkResult =
        function
        | Ok result -> Some result
        | _ -> None

    let unwrapErrorResult =
        function
        | Error errors -> Some errors
        | _ -> None

    let aggregateResults results =
        let allResultsSuccess =
            results
            |> List.forall (function
                | Ok _ -> true
                | _ -> false)

        if allResultsSuccess then
            Ok (results |> List.choose unwrapOkResult)
        else
            Error
                (results
                 |> List.choose unwrapErrorResult
                 |> List.concat)
