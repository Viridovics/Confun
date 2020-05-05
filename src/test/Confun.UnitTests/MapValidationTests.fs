namespace Confun.UnitTests

open Xunit
open FsUnit

open Confun.Core.Processing
open Confun.Core.Types

module MapValidationTests =
    let private haveErrorsCount errorsCount configMap =
        let validationResult = configMap |> MapValidator.validate
        match validationResult with
        | Error errorList -> errorList |> should haveLength errorsCount
        | _ -> UnitTests.testFail (sprintf "validation result is not error. Actual result %A" validationResult)

    let private isValid configMap =
        let validationResult = configMap |> MapValidator.validate
        match validationResult with
        | Ok(ValidatedConfunMap validatedConfigMap) -> should equivalent configMap validatedConfigMap
        | _ -> UnitTests.testFail (sprintf "validation result is not OK. Actual result %A" validationResult)

    [<Fact>]
    let ``Empty config map is valid``() =
        let configMap = []
        configMap |> isValid

    [<Fact>]
    let ``Config with duplicate names in different group is valid``() =
        let configMap =
            [ "RepeatingName", Port 42us
              "Group1", Group [ "RepeatingName", Port 42us ]
              "Group2", Group [ "RepeatingName", Port 42us ] ]
        configMap |> isValid

    [<Fact>]
    let ``Config with duplicate names is invalid``() =
        let configMap =
            [ "RepeatingName", Port 10us
              "RepeatingName", Str "10us" ]
        configMap |> haveErrorsCount 1

    [<Fact>]
    let ``Config with duplicate names in group is invalid``() =
        let configMap =
            [ "RepeatingName", Port 10us
              "Group",
              Group
                  [ "RepeatingName", Str "43"
                    "NameNormal", Str "42"
                    "RepeatingName", Port 90us ] ]
        configMap |> haveErrorsCount 1

    [<Fact>]
    let ``Config with empty name option in group is invalid``() =
        let configMap =
            [ "RepeatingName", Port 10us
              "Group",
              Group
                  [ "Name1", Str "43"
                    "Name2", Str "42"
                    "", Port 90us ] ]
        configMap |> haveErrorsCount 1

    [<Fact>]
    let ``Config with empty name option in root is invalid``() =
        let configMap =
            [ "\t", Port 10us
              "RepeatingName", Port 10us
              "  ",
              Group
                  [ "Name1", Str "43"
                    "Name2", Str "42"]]
        configMap |> haveErrorsCount 2

    [<Fact>]
    let ``Config with empty name option in group in array is invalid``() =
        let configMap =
            [ 
              "Port", Port 10us
              "GroupArray", Array
                  [| Group [
                      "RepeatingName", Port 10us
                      "RepeatingName", Port 10us]
                  |]
            ]
        configMap |> haveErrorsCount 1

    [<Fact>]
    let ``Config with null string option is invalid``() =
        let configMap =
            [
              "Port", Port 10us
              "GroupArray", Array
                  [| Group [
                      "Name1", Str null
                      "Name2", Port 10us]
                  |]
            ]
        configMap |> haveErrorsCount 1
