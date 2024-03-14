#r @"fsproj: ../../src/Morphir.CodeModel/Morphir.CodeModel.fsproj"
#r @"fsproj: ../../tests/Morphir.CodeModel.Tests/Morphir.CodeModel.Tests.fsproj"

open Morphir.IR
let aName = name { "Foo" }
printfn "aName:%A" aName
