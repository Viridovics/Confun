namespace Confun.Generator.Xml

open System.Xml.Linq

open Confun.Core.Processing.Api
open Confun.Core.Types

module XmlGenerator =
    let rec private convertConfigParamToXmlNode: ConfigParam -> XElement  =
        fun configParam ->
            let paramName, paramValue = configParam
            let paramXName = XName.Get paramName
            match paramValue with
            | Null -> XElement(paramXName)
            | Int i -> XElement(paramXName, i)
            | Float f -> XElement(paramXName, f)
            | Port port -> XElement(paramXName, port)
            | Str str -> XElement(paramXName, str)
            | NullableString str -> XElement(paramXName, str)
            | Regex (_, text) ->  XElement(paramXName, text)
            | Array arr ->
                let xmlNodes = arr |> Seq.map (fun p -> convertConfigParamToXmlNode ("Item", p))
                XElement(paramXName, xmlNodes)
            | Group group ->
                let xmlNodes = group |> Seq.map convertConfigParamToXmlNode
                XElement(paramXName, xmlNodes)

    let generator (xmlRoot: string): ConfigMapGenerator =
        fun (ValidatedConfunMap confunMap) ->
            let root = XElement(XName.Get xmlRoot)
            confunMap
            |> List.iter (convertConfigParamToXmlNode >> root.Add)
            root.ToString()
