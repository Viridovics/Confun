namespace Confun.UnitTests

open Xunit
open FsUnit

open Confun.Core.Processing
open Confun.Core.Types

exception ValidationTestsFailException of string

module MapValidationTests =
    let private validationTestsFail message  =
        raise (ValidationTestsFailException message)

    [<Fact>]
    let ``Empty config map is valid`` () =
        let configMap = []
        let validationResult = configMap |> MapValidator.validate
        match validationResult with
        | Ok (ValidatedConfunMap validatedConfigMap) -> should equivalent configMap validatedConfigMap
        | _ -> validationTestsFail (sprintf "validation result is not OK. Actual result %A" validationResult)

    [<Fact>]
    let ``Config with duplicate names is invalid`` () =
        let configMap = [
            "RepeatingName", Port 10us
            "RepeatingName", Str "10us"
        ]
        let validationResult = configMap |> MapValidator.validate
        match validationResult with
        | Error errorList -> errorList |> should haveLength 1
        | _ -> validationTestsFail (sprintf "validation result is not error. Actual result %A" validationResult)