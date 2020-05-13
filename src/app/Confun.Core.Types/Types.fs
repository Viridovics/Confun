namespace Confun.Core.Types

type ConfunMap = Dict

and Dict = ConfigParam list

and ConfigParam = string * ConfigValue

and ConfigValue =
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

type ValidationError = ValidationError of string

type ValidatedConfunMap = ValidatedConfunMap of ConfunMap

type ValidatedConfigFile =
    {
        Name: string
        DirectoryPath: string
        ValidatedParamsMap: ValidatedConfunMap
    }

type ConfigFile =
    {
        Name: string
        DirectoryPath: string
        ParamsMap: ConfunMap
    }

module ValidationError =
    let unwrap (ValidationError error) = error

    let addPrefixToErrors prefix errorList =
        errorList
        |> List.map
            (unwrap
             >> (sprintf "%s. %s" prefix)
             >> ValidationError)

module ValidatedConfunMap =
    let unwrap (ValidatedConfunMap configMap) = configMap

module Node =
    let createNode1 (nodeName: NodeName) (paramName: string) paramValue =
        Node (nodeName, [ (paramName, paramValue) ])

    let createNode2 (nodeName: NodeName) (param1Name: string) (param2Name: string) param1Value param2Value =
        Node (nodeName, [ param1Name, param1Value
                          param2Name, param2Value ])

    let createNode3 (nodeName: NodeName) (param1Name: string) (param2Name: string) (param3Name: string) param1Value param2Value param3Value =
        Node (nodeName, [ param1Name, param1Value
                          param2Name, param2Value
                          param3Name, param3Value ])
