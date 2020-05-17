namespace Confun.Core.Processing.Api

open Confun.Core.Types

type ValidationResult<'TError> =
    | Valid
    | Invalid of 'TError

type MapValidationResult = Result<ValidatedConfunMap, ConfunError list>

type OptionValidationResult = ValidationResult<ConfunError list>

type MapValidationStep = ConfunMap -> MapValidationResult

type ConfigParamValidationStep = ConfigParam -> OptionValidationResult

type ConfigMapGenerator = ValidatedConfunMap -> string