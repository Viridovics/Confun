namespace Confun.UnitTests.Generators

open System.Collections.Generic
open System.Xml.Linq

open Xunit
open FsUnit

open Confun.Core.Types
open Confun.Generator.Xml

module XmlGeneratorTests =
    [<Fact>]
    let ``Generate valid config to xml``() =
        let validatedConfunMap = ValidatedConfunMap [
            "IntValue", Int 100
            "FloatValue", Float 20.0
            "Port", Port 10us
            "String", Str "qwerty"
            "DatabaseConnection", Group [
                "Port", Port 10us
                "String", Str "qwerty"
            ]
            "SystemPorts", Array [| Port 40us; Port 80us; Port 8080us |]
            "NullVal", Null
            "NullStr", NullableString "Is not null"
            "Version", Regex (@"\d+\.\d+\.\d+\.\d+", "123.123.432.123")
        ]
        let xmlGenerator = XmlGenerator.generator "ConfigRoot"
        let xDocument = validatedConfunMap 
                                    |> xmlGenerator
                                    |> XDocument.Parse
        let root = xDocument.Root
        root.Name.LocalName |> should equal "ConfigRoot"
        root.Element(XName.Get "IntValue").Value |> int |> should equal 100
        root.Element(XName.Get "FloatValue").Value |> float |> should equal 20.0
        root.Element(XName.Get "Port").Value |> int |> should equal 10
        root.Element(XName.Get "String").Value |> should equal "qwerty"
        root.Element(XName.Get "SystemPorts").Elements() |> Seq.map (fun p -> p.Value |> int) |> should equivalent [| 40; 80; 8080 |]
        root.Element(XName.Get "NullVal").Value |> should equal ""
        root.Element(XName.Get "NullStr").Value |> should equal "Is not null"
        root.Element(XName.Get "Version").Value |> should equal "123.123.432.123"

        let group = root.Element(XName.Get "DatabaseConnection")
        group.Element(XName.Get "Port").Value |> int |> should equal 10
        group.Element(XName.Get "String").Value |> should equal "qwerty"


