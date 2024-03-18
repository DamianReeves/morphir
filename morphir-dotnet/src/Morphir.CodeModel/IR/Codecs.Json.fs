namespace Morphir.IR.Codecs.Json

type MorphirJsonFormatVersion =
    | V3
    | V5

    static member Default = V3

type MorphirJsonOptions =
    { FormatVersion: MorphirJsonFormatVersion }

    static member Default = { FormatVersion = MorphirJsonFormatVersion.Default }
    static member V3 = { FormatVersion = MorphirJsonFormatVersion.V3 }
    static member V5 = { FormatVersion = MorphirJsonFormatVersion.V5 }

    member o.WriteNamesAsArrays =
        match o.FormatVersion with
        | MorphirJsonFormatVersion.V3 -> true
        | MorphirJsonFormatVersion.V5 -> false

    member o.WriteNamesAsStrings = not o.WriteNamesAsArrays
