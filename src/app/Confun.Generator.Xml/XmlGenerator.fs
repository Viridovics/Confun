namespace Confun.Generator.Xml

open System.Xml.Linq

open Confun.Core.Processing.Api
open Confun.Core.Types

module XmlGenerator =
    let rec private convertConfigParamToXmlNode: ConfigParam -> XElement  =
        fun configParam ->
            let convertNodeToXml name (dict: Dict) =
                let nodeXName = XName.Get name
                let node = XElement(nodeXName)
                dict |> Seq.iter (fun (paramName, paramValue) ->
                                        let paramXName = XName.Get paramName
                                        match paramValue with
                                        | Null -> node.SetAttributeValue(paramXName, "")
                                        | Int i -> node.SetAttributeValue(paramXName, i)
                                        | Float f -> node.SetAttributeValue(paramXName, f)
                                        | Port port -> node.SetAttributeValue(paramXName, port)
                                        | Str str -> node.SetAttributeValue(paramXName, str)
                                        | NullableString str -> node.SetAttributeValue(paramXName, str)
                                        | Regex (_, text) ->  node.SetAttributeValue(paramXName, text)
                                        | _ -> node.Add(convertConfigParamToXmlNode (paramName, paramValue)))
                node

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
            | Node (name, dict) ->
                convertNodeToXml name dict

    let generator (xmlRoot: string): ConfigMapGenerator =
        fun (ValidatedConfunMap confunMap) ->
            let root = XElement(XName.Get xmlRoot)
            confunMap
            |> List.iter (convertConfigParamToXmlNode >> root.Add)
            root.ToString()
