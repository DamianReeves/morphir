namespace Morphir.Codecs

open Morphir.CodeModel.Versioning

type MorphirFormatOptions =
    | MorphirJson of MorphirJsonFormatOptions

    member o.IsTextFormat =
        match o with
        | MorphirJson _ -> true

    member o.Format: MorphirModelFormat =
        match o with
        | MorphirJson _ -> MorphirModelFormat.Text

and MorphirTextFormat = | Json

and MorphirBinaryFormat =
    | Cbor
    | MIR

and MorphirModelFormat =
    | Text
    | Binary

and MorphirJsonFormatOptions =
    { Version: Version
      NameEncodingMode: NameEncodingMode }

and NameEncodingMode =
    | AsArray
    | AsIdentifier
    | AsIri
