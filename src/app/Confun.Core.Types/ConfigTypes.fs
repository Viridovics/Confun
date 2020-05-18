namespace Confun.Core.Types

open System.Diagnostics.CodeAnalysis

type ConfunMap = Dict

and Dict = ConfigParam list

and ConfigParam = string * ConfigValue

and [<ExcludeFromCodeCoverage>] ConfigValue =
    | Null
    | Int of int32
    | Float of float
    | Port of uint16
    | Str of string
    | NullableString of string
    | Regex of RegexPattern * Text
    | Group of Dict
    | Array of ConfigValue array
    | Node of NodeName * Dict

and RegexPattern = string

and Text = string

and NodeName = string

[<ExcludeFromCodeCoverage>]
type ValidatedConfunMap = ValidatedConfunMap of ConfunMap

[<ExcludeFromCodeCoverage>]
type ValidatedConfigFile =
    { Name: string
      DirectoryPath: string
      ValidatedParamsMap: ValidatedConfunMap }

[<ExcludeFromCodeCoverage>]
type ConfigFile =
    { Name: string
      DirectoryPath: string
      ParamsMap: ConfunMap }


module ValidatedConfunMap =
    [<ExcludeFromCodeCoverage>]
    let unwrap (ValidatedConfunMap configMap) = configMap

module Node =
    let createNode1 (nodeName: NodeName) (paramName: string) paramValue =
        Node(nodeName, [ (paramName, paramValue) ])

    let createNode2 (nodeName: NodeName) (param1Name: string) (param2Name: string) param1Value param2Value =
        Node
            (nodeName,
             [ param1Name, param1Value
               param2Name, param2Value ])

    let createNode3
        (nodeName: NodeName)
        (param1Name: string)
        (param2Name: string)
        (param3Name: string)
        param1Value
        param2Value
        param3Value
        =
        Node
            (nodeName,
             [ param1Name, param1Value
               param2Name, param2Value
               param3Name, param3Value ])

    let createNode4
        (nodeName: NodeName)
        (param1Name: string)
        (param2Name: string)
        (param3Name: string)
        (param4Name: string)
        param1Value
        param2Value
        param3Value
        param4Value
        =
        Node
            (nodeName,
             [ param1Name, param1Value
               param2Name, param2Value
               param3Name, param3Value
               param4Name, param4Value ])
