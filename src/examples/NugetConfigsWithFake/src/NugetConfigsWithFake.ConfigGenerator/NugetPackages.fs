module NugetPackages

open Confun.Core.Types

let packageVersion versionStr = Regex(@"\d+\.\d+\.\d+", versionStr)

let packageNode = Node.createNode2 "PackageReference" "Include" "Version"

let NLogPackage = packageNode (Str "NLog") (packageVersion "4.7.1")

let NewtonSoftJsonPackage = packageNode (Str "Newtonsoft.Json") (packageVersion "12.0.3")