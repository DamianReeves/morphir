module Morphir.IR.Codecs.MorphirIRWriterContextTests

open Morphir.IR.Codecs
open Morphir.IR.Codecs.Json
open Morphir.IR.JsonCodecs
open Morphir.IR
open Morphir.IR.Name
open Morphir.SDK.Testing

[<Tests>]
let tests =
    let jsonTests =
        describe
            "JSON"
            [ test "Should be able to Convert a Name to JSON"
              <| fun _ ->
                  let name: Name = Name.fromString "MorphirValue"
                  let context: MorphirIRWriterContext = MorphirIRWriterContext.Default
                  let actual = context.ConvertToJson<MorphirJsonIRWriter, Name>(name)
                  actual |> Expect.equal """["morphir","value"]""" ]

    describe "MorphirIRWriterContext" [ jsonTests ]
