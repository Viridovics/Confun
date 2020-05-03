namespace Confun.Core.Types

type ConfunMap = Dict

and Dict = ConfigParam list

and ConfigParam = string * ConfigValue

and ConfigValue =
    | Port of uint16
    | Str of string
    | Group of Dict
    | Array of ConfigValue array

type ValidationError = ValidationError of string

type ValidatedConfunMap = ValidatedConfunMap of ConfunMap

type ConfigFile =
    {
        Name: string
        DirectoryPath: string
        ParamsMap: ValidatedConfunMap
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
