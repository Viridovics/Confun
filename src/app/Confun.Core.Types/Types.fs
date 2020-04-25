namespace Confun.Core.Types

type ConfunMap = 
    ConfigOption list
and ConfigOption =
    string * ConfigValue
and ConfigValue =
    | Port of uint16
    | Str of string
    | Group of ConfunMap


