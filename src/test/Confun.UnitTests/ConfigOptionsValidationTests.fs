namespace Confun.UnitTests

open Xunit
open FsUnit

open Confun.Core.Processing
open Confun.Core.Processing.Api
open Confun.Core.Types

module ConfigOptionsValidationTests =
    let private haveErrorsCountByValidationStep errorsCount configOptionValidationStep configOption =
        let validationResult = configOption |> configOptionValidationStep
        match validationResult with
        | Invalid errorList -> errorList |> should haveLength errorsCount
        | _ -> UnitTests.testFail (sprintf "validation result is not invalid. Actual result %A" validationResult)

    let private errorListContainsNameByValidationStep optionName configOptionValidationStep configOption =
        let validationResult = configOption |> configOptionValidationStep
        let escapedOptionName = sprintf "'%s'" optionName
        match validationResult with
        | Invalid errorList -> errorList |> List.map ValidationError.unwrap |> List.filter (fun s -> s.Contains escapedOptionName) |> should haveLength 1
        | _ -> UnitTests.testFail (sprintf "validation result is not invalid. Actual result %A" validationResult)

    let isValidByValidationStep configOptionValidationStep configOption =
        let validationResult = configOption |> configOptionValidationStep
        match validationResult with
        | Valid -> ()
        | Invalid errorList -> UnitTests.testFail (sprintf "validation result is error. Errors %A" errorList)

    [<Fact>]
    let ``Empty group is valid by validateNamesUniquenesInGroupOptionStep``() =
        let group = "", Group []
        group |> isValidByValidationStep ConfigOptionsValidation.validateNamesUniquenesInGroupOptionStep

    [<Fact>]
    let ``Group is valid by validateNamesUniquenesInGroupOptionStep``() =
        let group =
            "",
            Group
                [ "Name1", Port 1us
                  "Name2", Port 4us
                  "Name3", Port 1us ]
        group |> isValidByValidationStep ConfigOptionsValidation.validateNamesUniquenesInGroupOptionStep

    [<Fact>]
    let ``Group with duplicate names is invalid by validateNamesUniquenesInGroupOptionStep``() =
        let group =
            "",
            Group
                [ "RepeatingName", Str "1"
                  "RepeatingName2", Str "2"
                  "RepeatingName", Port 3us
                  "RepeatingName2", Str "4" ]
        group |> haveErrorsCountByValidationStep 2 ConfigOptionsValidation.validateNamesUniquenesInGroupOptionStep

    [<Fact>]
    let ``Group with duplicate names is invalid by validateNamesUniquenesInGroupOptionStep and error list contains non unique name``() =
        let group =
            "",
            Group
                [ "RepeatingName", Str "1"
                  "RepeatingName2", Str "2"
                  "NonRepeatingName", Port 3us
                  "RepeatingName", Port 3us
                  "RepeatingName2", Str "4" ]
        group |> haveErrorsCountByValidationStep 2 ConfigOptionsValidation.validateNamesUniquenesInGroupOptionStep
        group |> errorListContainsNameByValidationStep "RepeatingName" ConfigOptionsValidation.validateNamesUniquenesInGroupOptionStep
        group |> errorListContainsNameByValidationStep "RepeatingName2" ConfigOptionsValidation.validateNamesUniquenesInGroupOptionStep


