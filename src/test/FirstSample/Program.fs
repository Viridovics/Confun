// Learn more about F# at http://fsharp.org

open Confun.Core.Types

let m:ConfunMap = [
            "SrcPort", Port 10us
            "DestPort", Port 8080us
            "DatabaseConnection", Group [
                "ConnectionString", Str "ms-sql.localhost:9090"
            ]
        ]

[<EntryPoint>]
let main argv =
    printfn "%A" m
    0 // return an integer exit code
