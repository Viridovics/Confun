namespace Confun.UnitTests

open Xunit
open FsUnit

open Confun.Core.Types

module ConfigTypesTests =
    [<Fact>]
    let ``CreateNode1 function works`` () =
        let createdNode =
            Node.createNode1 "NodeName" "Attr1" (Str "123")

        createdNode
        |> should equal (Node("NodeName", [ "Attr1", (Str "123") ]))

    [<Fact>]
    let ``CreateNode2 function works`` () =
        let createdNode =
            Node.createNode2 "NodeName" "Attr1" "Attr2" (Str "123") (Port 80us)

        createdNode
        |> should equal
               (Node
                   ("NodeName",
                    [ "Attr1", (Str "123")
                      "Attr2", (Port 80us) ]))

    [<Fact>]
    let ``CreateNode3 function works`` () =
        let createdNode =
            Node.createNode3 "NodeName" "Attr1" "Attr2" "Attr3" (Str "123") (Port 80us) (Int 1100)

        createdNode
        |> should equal
               (Node
                   ("NodeName",
                    [ "Attr1", (Str "123")
                      "Attr2", (Port 80us)
                      "Attr3", (Int 1100) ]))

    [<Fact>]
    let ``CreateNode4 function works`` () =
        let createdNode =
            Node.createNode4 "NodeName" "Attr1" "Attr2" "Attr3" "Attr4" (Str "123") (Port 80us) (Int 1100) Null

        createdNode
        |> should equal
               (Node
                   ("NodeName",
                    [ "Attr1", (Str "123")
                      "Attr2", (Port 80us)
                      "Attr3", (Int 1100)
                      "Attr4", Null ]))
