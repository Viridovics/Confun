namespace Confun.Core.Processing.Api

open System.Diagnostics.CodeAnalysis

open Confun.Core.Types

[<ExcludeFromCodeCoverage>]
type ValidationResult<'TError> =
    | Valid
    | Invalid of 'TError

type MapValidationResult = Result<ValidatedConfunMap, ConfunError list>

type OptionValidationResult = ValidationResult<ConfunError list>

type MapValidationStep = ConfunMap -> MapValidationResult

type ConfigParamValidationStep = ConfigParam -> OptionValidationResult

type ConfigMapGenerator = ValidatedConfunMap -> string

type ValidationSteps = {
    MapSteps: MapValidationStep list
    ParamSteps: ConfigParamValidationStep list
}

type ValidationOptions =
    | MapValidationSteps of MapValidationStep list
    | ParamValidationSteps of ConfigParamValidationStep list
    | ValidationSteps of ValidationSteps

module ValidationOptions =
    let empty = (ValidationSteps { MapSteps = []; ParamSteps = [] })
// Bug with coverlet. Create issue. | ValidationOptions of {| MapSteps: MapValidationStep list; ParamSteps: ConfigParamValidationStep list |}