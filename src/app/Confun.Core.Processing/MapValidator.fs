namespace Confun.Core.Processing

open Confun.Core.Processing.Api
open Confun.Core.Types

module MapValidator =
    let private namesUniquenessValidationStep: MapValidationStep =
        fun configMap ->
            let validationResult = ConfigParamsValidation.validateOptionNamesUniquenesInList configMap
            match validationResult with
            | Valid ->
                configMap
                |> ValidatedConfunMap
                |> Ok
            | Invalid errorList -> Error(ValidationError.addPrefixToErrors "Error in-root of config map" errorList)

    let private namesEmptinessValidationStep: MapValidationStep =
        fun configMap ->
            let validationResult = ConfigParamsValidation.validateOptionNamesForEmptiness configMap
            match validationResult with
            | Valid ->
                configMap
                |> ValidatedConfunMap
                |> Ok
            | Invalid errorList -> Error(ValidationError.addPrefixToErrors "Error in-root of config map" errorList)

    let private configParamsValidationStep (optionValidationSteps: ConfigParamValidationStep list): MapValidationStep =
        fun configMap ->
            let optionValidation =
                fun configParam ->
                    optionValidationSteps
                    |> Seq.collect
                        ((fun step -> step configParam)
                         >> (function
                         | Valid -> []
                         | Invalid errorList -> errorList))

            let rec configParamsValidation (configParams: Dict) =
                let descendatErrors =
                    configParams
                    |> Seq.collect
                        ((function
                        | _, Group groupOptions -> configParamsValidation groupOptions
                        | arrayName, Array arr -> arr 
                                                    |> Array.mapi (fun index value -> ((sprintf "%s.[%d]" arrayName index)), value) 
                                                    |> Array.toList 
                                                    |> configParamsValidation
                        | _, Node (nodeName, dict) -> dict 
                                                    |> Seq.map (fun (paramName, value) -> ((sprintf "%s.%s" nodeName paramName)), value) 
                                                    |> Seq.toList 
                                                    |> configParamsValidation
                        | _ -> []))

                let innerErrors = configParams |> Seq.collect optionValidation
                [ innerErrors; descendatErrors ]
                |> Seq.concat
                |> Seq.toList

            let errorsList = configParamsValidation configMap
            if List.isEmpty errorsList then Ok(ValidatedConfunMap configMap) else Error errorsList

    let validate configMap: MapValidationResult =
        let optionConfigValidationSteps = [ ConfigParamsValidation.namesUniquenesInGroupValidationStep
                                            ConfigParamsValidation.namesEmptinessInGroupValidationStep
                                            ConfigParamsValidation.nodeNameEmptinessValidationStep
                                            ConfigParamsValidation.namesUniquenesInNodeValidationStep
                                            ConfigParamsValidation.namesEmptinessInNodeValidationStep
                                            ConfigParamsValidation.nullStringValidationStep
                                            ConfigParamsValidation.regexValidationStep ]

        let errorResults =
            [ namesUniquenessValidationStep
              namesEmptinessValidationStep
              (configParamsValidationStep optionConfigValidationSteps) ]
            |> List.collect
                ((fun step -> step configMap)
                 >> (function
                 | Ok _ -> []
                 | Error errorList -> errorList))
        if List.isEmpty errorResults then Ok(ValidatedConfunMap configMap) else Error errorResults
