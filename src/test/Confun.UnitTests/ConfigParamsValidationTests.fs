namespace Confun.UnitTests

open Xunit
open FsUnit

open Confun.Core.Processing
open Confun.Core.Processing.Api
open Confun.Core.Types

module ConfigParamsValidationTests =
    let private checkErrorListByValidationStep configParamValidationStep configParam erorrsListCheck =
        let validationResult = configParam |> configParamValidationStep
        match validationResult with
        | Invalid errorList ->
            errorList
            |> List.map ValidationError.unwrap
            |> erorrsListCheck
        | _ -> UnitTests.testFail (sprintf "validation result is not invalid. Actual result %A" validationResult)

    let private haveErrorsCountByValidationStep errorsCount configParamValidationStep configParam =
        checkErrorListByValidationStep configParamValidationStep configParam (should haveLength errorsCount)

    let private errorsListContainsNameByValidationStep optionName configParamValidationStep configParam =
        let escapedOptionName = sprintf "'%s'" optionName
        checkErrorListByValidationStep configParamValidationStep configParam (fun errorsList ->
            errorsList
            |> List.filter (fun s -> s.Contains escapedOptionName)
            |> should haveLength 1)

    let private allErrorsContainsNameByValidationStep optionName configParamValidationStep configParam =
        let escapedOptionName = sprintf "'%s'" optionName
        checkErrorListByValidationStep configParamValidationStep configParam (fun errorsList ->
            errorsList
            |> List.forall (fun s -> s.Contains escapedOptionName)
            |> should be True)

    let isValidByValidationStep configParamValidationStep configParam =
        let validationResult = configParam |> configParamValidationStep
        match validationResult with
        | Valid -> ()
        | Invalid errorList -> UnitTests.testFail (sprintf "validation result is error. Errors %A" errorList)

    [<Fact>]
    let ``Empty group is valid by validateNamesUniquenesInGroupOptionStep``() =
        let group = "", Group []
        group |> isValidByValidationStep ConfigParamsValidation.validateNamesUniquenesInGroupOptionStep

    [<Fact>]
    let ``Group is valid by validateNamesUniquenesInGroupOptionStep``() =
        let group =
            "GroupX",
            Group
                [ "Name1", Port 1us
                  "Name2", Port 4us
                  "Name3", Port 1us ]
        group |> isValidByValidationStep ConfigParamsValidation.validateNamesUniquenesInGroupOptionStep

    [<Fact>]
    let ``Group with duplicate names is invalid by validateNamesUniquenesInGroupOptionStep``() =
        let group =
            "GroupY",
            Group
                [ "RepeatingName", Str "1"
                  "RepeatingName2", Str "2"
                  "RepeatingName", Port 3us
                  "RepeatingName2", Str "4" ]
        group |> haveErrorsCountByValidationStep 2 ConfigParamsValidation.validateNamesUniquenesInGroupOptionStep

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
        group |> haveErrorsCountByValidationStep 2 ConfigParamsValidation.validateNamesUniquenesInGroupOptionStep
        group
        |> errorsListContainsNameByValidationStep "RepeatingName"
               ConfigParamsValidation.validateNamesUniquenesInGroupOptionStep
        group
        |> errorsListContainsNameByValidationStep "RepeatingName2"
               ConfigParamsValidation.validateNamesUniquenesInGroupOptionStep
        group
        |> allErrorsContainsNameByValidationStep "GroupZ"
               ConfigParamsValidation.validateNamesUniquenesInGroupOptionStep

    [<Fact>]
    let ``Group with empty name option is invalid by validateNamesEmptinessInGroupOptionStep``() =
        let group =
            "GroupWithEmptyNameOption",
            Group
                [ "Name", Str "1"
                  "Name2", Str "2"
                  "    \t   ", Port 3us
                  "Name3", Str "4" ]
        group |> haveErrorsCountByValidationStep 1 ConfigParamsValidation.validateNamesEmptinessInGroupOptionStep
        group
        |> allErrorsContainsNameByValidationStep "GroupWithEmptyNameOption"
               ConfigParamsValidation.validateNamesEmptinessInGroupOptionStep

    [<Fact>]
    let ``Group with null name option is invalid by validateNamesEmptinessInGroupOptionStep``() =
        let group =
            "GroupWithNullNameOption",
            Group
                [ "Name", Str "1"
                  "Name2", Str "2"
                  null, Port 3us
                  "Name3", Str "4" ]
        group |> haveErrorsCountByValidationStep 1 ConfigParamsValidation.validateNamesEmptinessInGroupOptionStep
        group
        |> allErrorsContainsNameByValidationStep "GroupWithNullNameOption"
               ConfigParamsValidation.validateNamesEmptinessInGroupOptionStep

