namespace Morphir.Codecs.SystemTextJson

open Morphir.Codecs
open Fleece
open Fleece.SystemTextJson

[<AutoOpen>]
module Operations =
    let encoder = Fleece.SystemTextJson.Encoding
    let inline toJson (options: MorphirJsonFormatOptions) value = 
        //let encoder = encoder
        failwith "Not implemented"
    