namespace Morphir.Codecs.Json

open Morphir.Codecs

[<AbstractClass>]
type MorphirJsonWriter(jsonOptions: MorphirJsonFormatOptions) =
    inherit MorphirWriter(MorphirFormatOptions.MorphirJson(jsonOptions))
    member this.JsonOptions = jsonOptions
