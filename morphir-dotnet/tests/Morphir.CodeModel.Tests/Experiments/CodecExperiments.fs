namespace Morphir.Codec
open Fleece

module Json =

    type MorphirJsonOptions =
        | Version3
        static member Default: MorphirJsonOptions = Version3
        
    type Label =
        | Label of string
        static member ToJson(Label text, _:MorphirJsonOptions) = JString text 

    type MorphirJsonCodec() =
        static member inline Encode(value:^Value, options:MorphirJsonOptions) =
            ((^Value : (static member Encode : ^Value * MorphirJsonOptions -> string) (value, options)) : string)

        static member inline Encode(value:^Value) = MorphirJsonCodec.Encode(value, MorphirJsonOptions.Default)

module CodecDemoTests =
    open Json
    open Xunit
    open FsUnit
    open FsUnit.Xunit
    open Fleece.SystemTextJson
    
    [<Fact>]
    let ``Encoding should work``() =
        let value = Label "Hello"
        let actual = (string (toJson value))
        actual |> should equal "\"Hello\""
        