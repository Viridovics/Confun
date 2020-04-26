namespace Confun.Core.Api

open Confun.Core.Types

type ValidationResult =
    | ErrorValidation of ValidationError
    | SuccessValidation of ValidatedConfunMap
and
    ValidationError = ValidationError of string
and 
    ValidatedConfunMap = private ValidatedConfunMap of ConfunMap

module MapValidator = 
    let validate (configMap: ConfunMap) : ValidationResult = 
        SuccessValidation (ValidatedConfunMap configMap)