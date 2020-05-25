# Confun

## What is Confun
Confun is a system for describing, validating and generating config files usings F#.

## Quick start

* Create empty F# project (e.g. dotnet new console -lang F# -o ConfigGenerator)
* Add to this project nuget package ...
* Describe your config in program.fs

        open Confun.Core.Types
        
        let configMap = [
            "AppPort", Port 8080us
            "DatabaseConnection", Group [
                "Instance", Str "localhost:8080"
                "User", Str "UserName"
                "Pwd", Null // Fill in runtime or deployment
                ]
            ]

        let config = {
            Name = "config.json"
            DirectoryPath = ".\src\App\Config"
            ParamsMap = configMap 
        }

* Add code for config validation and generation

        open Confun.Core.Processing
        open Confun.Generator.Json

        let (||>>) a b = Result.bind b a

        [<EntryPoint>]
        let main argv =
            let result = config 
                            |> ConfigValidator.validate
                            ||>> (ConfigGenerator.generate JsonGenerator.generator)
            match result with
            | Ok messages ->
                printfn "%A" messages
                0
            | Error errors ->
                printfn "%A" errors
                1

* Run ConfigGenerator.fsproj
* Full example [here](https://github.com/Viridovics/Confun/tree/master/src/examples/ReadmeExample)

## License
Copyright (c) Viridovics

Licensed under the [MIT](LICENSE) License.

## Credits
Confun examples, unit tests and generator module use Json library ([Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json)) under [MIT License](https://github.com/JamesNK/Newtonsoft.Json/blob/master/LICENSE.md)

Confun examples, unit tests and generator module use Yaml library ([YamlDotNet](https://github.com/aaubry/YamlDotNet)) under [MIT License](https://github.com/aaubry/YamlDotNet/blob/master/LICENSE.txt)
