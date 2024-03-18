namespace Morphir.IR

open Morphir.IR.Codecs.Json
open System.Collections.Concurrent

type ContextProperties private (properties: ConcurrentDictionary<ContextProperty, obj>) =
    new() = ContextProperties(ConcurrentDictionary<ContextProperty, obj>())

    member _.Get(property: ContextProperty<'Value>) : Option<'Value> =
        match properties.TryGetValue(property) with
        | true, (:? 'Value as value) -> value |> Some
        | _ -> None

    member this.GetOrDefault(property: ContextProperty<'Value>, defaultValue: 'Value) : 'Value =
        this.Get property |> Option.defaultValue (defaultValue)

    member this.GetOrDefault(property: ContextProperty<'Value>, defaultValueProvider: unit -> 'Value) : 'Value =
        this.Get property |> Option.defaultValue (defaultValueProvider ())

    member _.Set(property: ContextProperty<'Value>, value: 'Value) =
        let valueAsObj = value :> obj
        properties.AddOrUpdate(property, valueAsObj, (fun _ _ -> valueAsObj)) |> ignore

and ContextProperty =
    interface
        abstract member Name: string
    end

and IHaveDefault<'Value> =
    abstract member Default: 'Value

and ContextProperty<'Value> =
    inherit ContextProperty

module ContextProperties =
    let JsonFormatVersion: ContextProperty<MorphirJsonFormatVersion> =
        { new ContextProperty<MorphirJsonFormatVersion> with
            member _.Name = "JsonFormatVersion" }

    let MorphirJsonOptions: ContextProperty<MorphirJsonOptions> =
        { new ContextProperty<MorphirJsonOptions> with
            member _.Name = "MorphirJsonOptions" }

namespace Morphir.IR.Codecs

open Morphir.IR
type MorphirIRWriterContext = { Properties: ContextProperties }

namespace Morphir.IR.Codecs.Json

open Morphir.IR
open Morphir.IR.Codecs

[<AutoOpen>]
module JsonHelpers =
    let getMorphirJsonOptions (context: MorphirIRWriterContext) : MorphirJsonOptions =
        context.Properties.GetOrDefault<MorphirJsonOptions>(
            ContextProperties.MorphirJsonOptions,
            MorphirJsonOptions.Default
        )
