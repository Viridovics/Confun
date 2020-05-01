namespace Confun.Core.Processing

open Confun.Core.Processing.Api
open Confun.Core.Types

module MapValidator =
    let private namesUniquenessValidationStep: MapValidationStep =
        fun configMap ->
            let validationResult = ConfigOptionsValidation.validateOptionNamesUniquenesInList configMap
            match validationResult with
            | Valid ->
                configMap
                |> ValidatedConfunMap
                |> Ok
            | Invalid errorList -> Error(ValidationError.addPrefixToErrors "Error in-root of config map" errorList)

    let private namesEmptinessValidationStep: MapValidationStep =
        fun configMap ->
            let validationResult = ConfigOptionsValidation.validateOptionNamesForEmptiness configMap
            match validationResult with
            | Valid ->
                configMap
                |> ValidatedConfunMap
                |> Ok
            | Invalid errorList -> Error(ValidationError.addPrefixToErrors "Error in-root of config map" errorList)

    let private configOptionsValidationStep (optionValidationSteps: ConfigOptionValidationStep list): MapValidationStep =
        fun configMap ->
            let optionValidation =
                fun configOption ->
                    optionValidationSteps
                    |> Seq.collect
                        ((fun step -> step configOption)
                         >> (function
                         | Valid -> []
                         | Invalid errorList -> errorList))

            let rec configOptionsValidation (configOptions: ConfigOption list) =
                let descendatErrors =
                    configOptions
                    |> Seq.collect
                        ((function
                        | _, Group groupOptions -> configOptionsValidation groupOptions
                        | _ -> []))

                let innerErrors = configOptions |> Seq.collect optionValidation
                [ innerErrors; descendatErrors ]
                |> Seq.concat
                |> Seq.toList

            let errorsList = configOptionsValidation configMap
            if List.isEmpty errorsList then Ok(ValidatedConfunMap configMap) else Error errorsList

    let validate configMap: MapValidationResult =
        let optionConfigValidationSteps = [ ConfigOptionsValidation.validateNamesUniquenesInGroupOptionStep;
                                            ConfigOptionsValidation.validateNamesEmptinessInGroupOptionStep ]

        let errorResults =
            [ namesUniquenessValidationStep
              namesEmptinessValidationStep
              (configOptionsValidationStep optionConfigValidationSteps) ]
            |> List.collect
                ((fun step -> step configMap)
                 >> (function
                 | Ok _ -> []
                 | Error errorList -> errorList))
        if List.isEmpty errorResults then Ok(ValidatedConfunMap configMap) else Error errorResults
