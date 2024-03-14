namespace Morphir.Host

open System

type Location =
    | Local of Path: string
    | Remote of LocationUri: Uri

module private HostConfigurationInternal =

    let countVowelsExtensionLocation =
        Uri("https://github.com/extism/plugins/releases/latest/download/count_vowels.wasm")
        |> Remote

type HostConfiguration =
    { Extensions: HostExtensionsConfiguration }

    static member Default =
        { Extensions = { ExtensionsSearchLocations = [ HostConfigurationInternal.countVowelsExtensionLocation ] } }

and HostExtensionsConfiguration =
    { ExtensionsSearchLocations: Location list }

module HostConfiguration =
    let Default = HostConfiguration.Default
