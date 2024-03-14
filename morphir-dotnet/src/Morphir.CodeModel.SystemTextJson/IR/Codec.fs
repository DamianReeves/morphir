namespace Morphir.IR

open Morphir.IR.Name
open System.Text.Json
open System.Text.Json.Serialization

module Codec =
    let DefaultJsonOptions = JsonFSharpOptions.Default()



    module Name =

        let toJsonWithOptions (name: Name) (options: JsonFSharpOptions) =
            let resolvedOptions = options.ToJsonSerializerOptions()
            JsonSerializer.Serialize(name, resolvedOptions)

        let inline toJson (name: Name) =
            toJsonWithOptions name DefaultJsonOptions
