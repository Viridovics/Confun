namespace Confun.UnitTests

open Xunit
open FsUnit

open Confun.Core.Types

module ErrorsTypesTests =
    [<Fact>]
    let ``Add prefix to validation error`` () =
        let validationError = ValidationError "Error"

        let errorWithPrefix =
            validationError
            |> ConfunError.addPrefixToError "Prefix"

        errorWithPrefix
        |> ConfunError.toString
        |> should equal "Prefix. Error"

    [<Fact>]
    let ``Add prefix to generation error`` () =
        let generationError = GenerationError "Error"

        let errorWithPrefix =
            generationError
            |> ConfunError.addPrefixToError "Prefix"

        errorWithPrefix
        |> ConfunError.toString
        |> should equal "Prefix. Error"

    [<Fact>]
    let ``Unwrap result functions work correct`` () =
        Ok "1"
        |> ConfunError.unwrapOkResult
        |> should equal (Some "1")
        Error "1"
        |> ConfunError.unwrapOkResult
        |> should equal None
        Ok "1"
        |> ConfunError.unwrapErrorResult
        |> should equal None
        Error "1"
        |> ConfunError.unwrapErrorResult
        |> should equal (Some "1")

    [<Fact>]
    let ``List of ok results aggregates to Ok of list results`` () =
        [ Ok "1"; Ok "2" ]
        |> ConfunError.aggregateResults
        |> ConfunError.unwrapOkResult
        |> should equal (Some [ "1"; "2" ])

    [<Fact>]
    let ``List of Ok/Error results aggregates to Error of list results`` () =
        [ Ok "1"
          Ok "2"
          Error [ "3"; "4" ]
          Ok "5"
          Error [ "6"; "7" ] ]
        |> ConfunError.aggregateResults
        |> ConfunError.unwrapErrorResult
        |> should equal (Some [ "3"; "4"; "6"; "7" ])

    [<Fact>]
    let ``List of empty results aggregates to Ok of list results`` () =
        []
        |> ConfunError.aggregateResults
        |> ConfunError.unwrapOkResult
        |> should equal (Some [])
