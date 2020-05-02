namespace Confun.Core.Processing.Api

open Confun.Core.Types

type ValidationResult<'TError> =
    | Valid
    | Invalid of 'TError

type MapValidationResult = Result<ValidatedConfunMap, ValidationError list>

type OptionValidationResult = ValidationResult<ValidationError list>

type MapValidationStep = ConfunMap -> MapValidationResult

type ConfigOptionValidationStep = ConfigOption -> OptionValidationResult

type ConfigGenerator = ValidatedConfunMap -> string