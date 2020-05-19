namespace Confun.UnitTests

open Xunit
open FsUnit

open Confun.Core.Processing
open Confun.Core.Processing.Api
open Confun.Core.Types

module ConfigValidationTests =
    let private isValid config =
        let validationResult = config |> ConfigValidator.validate
        match validationResult with
        | Ok validatedConfigFile ->
            validatedConfigFile |> should equal
                                    { Name = config.Name
                                      DirectoryPath = config.DirectoryPath
                                      ValidatedParamsMap = ValidatedConfunMap config.ParamsMap }
        | _ -> UnitTests.testFail (sprintf "validation result is not OK. Actual result %A" validationResult)

    let private haveErrorsCount errorsCount config =
        let validationResult = config |> ConfigValidator.validate
        match validationResult with
        | Error errorList -> errorList |> should haveLength errorsCount
        | _ -> UnitTests.testFail (sprintf "validation result is not error. Actual result %A" validationResult)

    [<Fact>]
    let ``Config with empty map is valid``() =
        let config =  {Name = "config.Name"
                       DirectoryPath = "config.DirectoryPath"
                       ParamsMap = [] }
        config |> isValid

    [<Fact>]
    let ``Config with valid map is valid``() =
        let config = { Name = "config.Name"
                       DirectoryPath = "config.DirectoryPath"
                       ParamsMap = [ "RepeatingName", Port 42us
                                     "Group1", Group [ "RepeatingName", Port 42us ]
                                     "Group2", Group [ "RepeatingName", Port 42us ] ] }
        config |> isValid

    [<Fact>]
    let ``Configs with valid map is valid``() =
        let config1 = { Name = "config.Name"
                        DirectoryPath = "config.DirectoryPath"
                        ParamsMap = [ "RepeatingName", Port 42us
                                      "Group1", Group [ "RepeatingName", Port 42us ]
                                      "Group2", Group [ "RepeatingName", Port 42us ] ] }
        let config2 = { Name = "config2.Name"
                        DirectoryPath = "config.DirectoryPath"
                        ParamsMap = [ "RepeatingName", Port 42us
                                      "Group1", Group [ "RepeatingName", Port 42us ]
                                      "Group2", Group [ "RepeatingName", Port 42us ] ] }
        
        let validatedConfig1 = { Name = config1.Name
                                 DirectoryPath = config1.DirectoryPath
                                 ValidatedParamsMap = ValidatedConfunMap config1.ParamsMap }
        
        let validatedConfig2 = { Name = config2.Name
                                 DirectoryPath = config2.DirectoryPath
                                 ValidatedParamsMap = ValidatedConfunMap config2.ParamsMap }

        let validatedConfigsResult = [ config1; config2 ] |> ConfigValidator.validateAll

        match validatedConfigsResult with
        | Ok validatedConfigs -> validatedConfigs |> should equivalent [validatedConfig1; validatedConfig2]
        | _ -> UnitTests.testFail (sprintf "validation result is not OK. Actual result %A" validatedConfigsResult)

    [<Fact>]
    let ``Configs with valid map is valid (check validateWith)``() =
        let everyMapIsBad: MapValidationStep =
            fun _ -> Error( [ ValidationError "Map is bad!" ])
        let config1 = { Name = "config.Name"
                        DirectoryPath = "config.DirectoryPath"
                        ParamsMap = [ "RepeatingName", Port 42us
                                      "Group1", Group [ "RepeatingName", Port 42us ]
                                      "Group2", Group [ "RepeatingName", Port 42us ] ] }
        let config2 = { Name = "config2.Name"
                        DirectoryPath = "config.DirectoryPath"
                        ParamsMap = [ "RepeatingName", Port 42us
                                      "Group1", Group [ "RepeatingName", Port 42us ]
                                      "Group2", Group [ "RepeatingName", Port 42us ] ] }
        
        let validatedConfig1 = { Name = config1.Name
                                 DirectoryPath = config1.DirectoryPath
                                 ValidatedParamsMap = ValidatedConfunMap config1.ParamsMap }
        
        let validatedConfig2 = { Name = config2.Name
                                 DirectoryPath = config2.DirectoryPath
                                 ValidatedParamsMap = ValidatedConfunMap config2.ParamsMap }

        let validatedConfigsResult = [ config1; config2 ] |> (ConfigValidator.validateAllWith (MapValidationSteps [ everyMapIsBad ]))
        match validatedConfigsResult with
        | Error errorList -> errorList |> should haveLength 2
        | _ -> UnitTests.testFail (sprintf "validation result is not error. Actual result %A" validatedConfigsResult)

    [<Fact>]
    let ``Config with invalid map is invalid``() =
        let config = { Name = "config.Name"
                       DirectoryPath = "config.DirectoryPath"
                       ParamsMap = [ "Version", Regex(@"\d+\.\d+\.\d+\.\d+", "123.123.432.") ] }
        config |> haveErrorsCount 1
