namespace Confun.UnitTests

open Xunit
open FsUnit

open Confun.Core.Processing
open Confun.Core.Processing.Api
open Confun.Core.Types

module ConfigOptionsValidationTests =
    let private checkErrorListByValidationStep configOptionValidationStep configOption erorrsListCheck =
        let validationResult = configOption |> configOptionValidationStep
        match validationResult with
        | Invalid errorList ->
            errorList
            |> List.map ValidationError.unwrap
            |> erorrsListCheck
        | _ -> UnitTests.testFail (sprintf "validation result is not invalid. Actual result %A" validationResult)

    let private haveErrorsCountByValidationStep errorsCount configOptionValidationStep configOption =
        checkErrorListByValidationStep configOptionValidationStep configOption (should haveLength errorsCount)

    let private errorsListContainsNameByValidationStep optionName configOptionValidationStep configOption =
        let escapedOptionName = sprintf "'%s'" optionName
        checkErrorListByValidationStep configOptionValidationStep configOption (fun errorsList ->
            errorsList
            |> List.filter (fun s -> s.Contains escapedOptionName)
            |> should haveLength 1)

    let private allErrorsContainsNameByValidationStep optionName configOptionValidationStep configOption =
        let escapedOptionName = sprintf "'%s'" optionName
        checkErrorListByValidationStep configOptionValidationStep configOption (fun errorsList ->
            errorsList
            |> List.forall (fun s -> s.Contains escapedOptionName)
            |> should be True)

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
            "GroupX",
            Group
                [ "Name1", Port 1us
                  "Name2", Port 4us
                  "Name3", Port 1us ]
        group |> isValidByValidationStep ConfigOptionsValidation.validateNamesUniquenesInGroupOptionStep

    [<Fact>]
    let ``Group with duplicate names is invalid by validateNamesUniquenesInGroupOptionStep``() =
        let group =
            "GroupY",
            Group
                [ "RepeatingName", Str "1"
                  "RepeatingName2", Str "2"
                  "RepeatingName", Port 3us
                  "RepeatingName2", Str "4" ]
        group |> haveErrorsCountByValidationStep 2 ConfigOptionsValidation.validateNamesUniquenesInGroupOptionStep

    [<Fact>]
    let ``Group duplicate names is contained in error list``() =
        let group =
            "GroupZ",
            Group
                [ "RepeatingName", Str "1"
                  "RepeatingName2", Str "2"
                  "NonRepeatingName", Port 3us
                  "RepeatingName", Port 3us
                  "RepeatingName2", Str "4" ]
        group |> haveErrorsCountByValidationStep 2 ConfigOptionsValidation.validateNamesUniquenesInGroupOptionStep
        group
        |> errorsListContainsNameByValidationStep "RepeatingName"
               ConfigOptionsValidation.validateNamesUniquenesInGroupOptionStep
        group
        |> errorsListContainsNameByValidationStep "RepeatingName2"
               ConfigOptionsValidation.validateNamesUniquenesInGroupOptionStep
        group
        |> allErrorsContainsNameByValidationStep "GroupZ"
               ConfigOptionsValidation.validateNamesUniquenesInGroupOptionStep

    [<Fact>]
    let ``Group with empty name option is invalid by validateNamesEmptinessInGroupOptionStep``() =
        let group =
            "GroupWithEmptyNameOption",
            Group
                [ "Name", Str "1"
                  "Name2", Str "2"
                  "    \t   ", Port 3us
                  "Name3", Str "4" ]
        group |> haveErrorsCountByValidationStep 1 ConfigOptionsValidation.validateNamesEmptinessInGroupOptionStep
        group
        |> allErrorsContainsNameByValidationStep "GroupWithEmptyNameOption"
               ConfigOptionsValidation.validateNamesEmptinessInGroupOptionStep

    [<Fact>]
    let ``Group with null name option is invalid by validateNamesEmptinessInGroupOptionStep``() =
        let group =
            "GroupWithNullNameOption",
            Group
                [ "Name", Str "1"
                  "Name2", Str "2"
                  null, Port 3us
                  "Name3", Str "4" ]
        group |> haveErrorsCountByValidationStep 1 ConfigOptionsValidation.validateNamesEmptinessInGroupOptionStep
        group
        |> allErrorsContainsNameByValidationStep "GroupWithNullNameOption"
               ConfigOptionsValidation.validateNamesEmptinessInGroupOptionStep

