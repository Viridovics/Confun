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
