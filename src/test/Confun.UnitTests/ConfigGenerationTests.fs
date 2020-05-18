namespace Confun.UnitTests

open System.IO

open Xunit
open FsUnit

open Confun.Core.Processing
open Confun.Core.Types
open Confun.Generator.Json

module ConfigGenerationTests =
    let private isValid partOfPath =
        function
        | Ok message -> message |> should contain partOfPath
        | Error error -> UnitTests.testFail (sprintf "validation result is not OK. Actual result %A" error)

    let private isInvalid partOfErrorMessage =
        function
        | Error errors ->
            errors
            |> (sprintf "%A")
            |> should contain partOfErrorMessage
        | Ok message -> UnitTests.testFail (sprintf "validation result is OK. Actual result %A" message)

    [<Fact>]
    let ``Empty config is generated`` () =
        let config =
            { Name = "./testConfig.json"
              DirectoryPath = "config.DirectoryPath"
              ValidatedParamsMap = ValidatedConfunMap [] }

        config
        |> ConfigGenerator.generate JsonGenerator.generator
        |> isValid "testConfig.json"
        File.Exists "./config.DirectoryPath/testConfig.json"
        |> should be True

    [<Fact>]
    let ``Empty configs is generated`` () =
        let config1 =
            { Name = "./testConfig1.json"
              DirectoryPath = "config.DirectoryPath"
              ValidatedParamsMap = ValidatedConfunMap [] }

        let config2 =
            { Name = "./testConfig2.json"
              DirectoryPath = "config.DirectoryPath"
              ValidatedParamsMap = ValidatedConfunMap [] }

        let generationResult =
            [ config1; config2 ]
            |> ConfigGenerator.generateAll JsonGenerator.generator

        match generationResult with
        | Ok [ res1; res2 ] ->
            Ok res1 |> isValid "testConfig1.json"
            Ok res2 |> isValid "testConfig2.json"
        | _ -> UnitTests.testFail (sprintf "generation result is not OK. Actual result %A" generationResult)

        File.Exists "./config.DirectoryPath/testConfig1.json"
        |> should be True
        File.Exists "./config.DirectoryPath/testConfig2.json"
        |> should be True

    [<Fact>]
    let ``Empty config with invalid path is not generated`` () =
        let config =
            { Name = "./testConfig>.json"
              DirectoryPath = "config.DirectoryPath"
              ValidatedParamsMap = ValidatedConfunMap [] }

        config
        |> ConfigGenerator.generate JsonGenerator.generator
        |> isInvalid "testConfig>.json"
