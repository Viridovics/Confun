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
    let ``Empty group is valid by namesUniquenesInGroupValidationStep`` () =
        let group = "", Group []
        group
        |> isValidByValidationStep ConfigParamsValidation.namesUniquenesInGroupValidationStep

    [<Fact>]
    let ``Group is valid by namesUniquenesInGroupValidationStep`` () =
        let group =
            "GroupX",
            Group
                [ "Name1", Port 1us
                  "Name2", Port 4us
                  "Name3", Port 1us ]

        group
        |> isValidByValidationStep ConfigParamsValidation.namesUniquenesInGroupValidationStep

    [<Fact>]
    let ``Group with duplicate names is invalid by namesUniquenesInGroupValidationStep`` () =
        let group =
            "GroupY",
            Group
                [ "RepeatingName", Str "1"
                  "RepeatingName2", Str "2"
                  "RepeatingName", Port 3us
                  "RepeatingName2", Str "4" ]

        group
        |> haveErrorsCountByValidationStep 2 ConfigParamsValidation.namesUniquenesInGroupValidationStep

    [<Fact>]
    let ``Group duplicate names is contained in error list`` () =
        let group =
            "GroupZ",
            Group
                [ "RepeatingName", Str "1"
                  "RepeatingName2", Str "2"
                  "NonRepeatingName", Port 3us
                  "RepeatingName", Port 3us
                  "RepeatingName2", Str "4" ]

        group
        |> haveErrorsCountByValidationStep 2 ConfigParamsValidation.namesUniquenesInGroupValidationStep
        group
        |> errorsListContainsNameByValidationStep "RepeatingName"
               ConfigParamsValidation.namesUniquenesInGroupValidationStep
        group
        |> errorsListContainsNameByValidationStep "RepeatingName2"
               ConfigParamsValidation.namesUniquenesInGroupValidationStep
        group
        |> allErrorsContainsNameByValidationStep "GroupZ" ConfigParamsValidation.namesUniquenesInGroupValidationStep

    [<Fact>]
    let ``Group with empty name option is invalid by namesEmptinessInGroupValidationStep`` () =
        let group =
            "GroupWithEmptyNameOption",
            Group
                [ "Name", Str "1"
                  "Name2", Str "2"
                  "    \t   ", Port 3us
                  "Name3", Str "4" ]

        group
        |> haveErrorsCountByValidationStep 1 ConfigParamsValidation.namesEmptinessInGroupValidationStep
        group
        |> allErrorsContainsNameByValidationStep "GroupWithEmptyNameOption"
               ConfigParamsValidation.namesEmptinessInGroupValidationStep

    [<Fact>]
    let ``Group with null name option is invalid by namesEmptinessInGroupValidationStep`` () =
        let group =
            "GroupWithNullNameOption",
            Group
                [ "Name", Str "1"
                  "Name2", Str "2"
                  null, Port 3us
                  "Name3", Str "4" ]

        group
        |> haveErrorsCountByValidationStep 1 ConfigParamsValidation.namesEmptinessInGroupValidationStep
        group
        |> allErrorsContainsNameByValidationStep "GroupWithNullNameOption"
               ConfigParamsValidation.namesEmptinessInGroupValidationStep

    [<Fact>]
    let ``Null string is invalid by nullStringValidationStep`` () =
        let nullString = "NullString", Str null
        nullString
        |> haveErrorsCountByValidationStep 1 ConfigParamsValidation.nullStringValidationStep
        nullString
        |> allErrorsContainsNameByValidationStep "NullString" ConfigParamsValidation.nullStringValidationStep

    [<Fact>]
    let ``Version is valid by regexValidationStep`` () =
        let validParam = "Version", Regex (@"\d+\.\d+\.\d+\.\d+", "123.123.432.123")
        validParam |> isValidByValidationStep ConfigParamsValidation.regexValidationStep

    [<Fact>]
    let ``Invalid version is checked by regexValidationStep`` () =
        let invalidParam = "Version", Regex (@"\d+\.\d+\.\d+\.\d+", "123.123.432.")
        invalidParam |> haveErrorsCountByValidationStep 1 ConfigParamsValidation.regexValidationStep

        invalidParam |> allErrorsContainsNameByValidationStep "123.123.432." ConfigParamsValidation.regexValidationStep
        invalidParam |> allErrorsContainsNameByValidationStep "Version" ConfigParamsValidation.regexValidationStep
        invalidParam |> allErrorsContainsNameByValidationStep @"^\d+\.\d+\.\d+\.\d+$" ConfigParamsValidation.regexValidationStep

//TODO: Add tests to Node rules