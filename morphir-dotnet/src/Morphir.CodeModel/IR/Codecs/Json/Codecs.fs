namespace Morphir.IR.Codecs.Json

open Morphir.IR
open Morphir.IR.Codecs
open System.IO

// module Name =
//     open Morphir.IR.Name


//     let writeJsonToWriter (writer: BufferWriter) (name: Name) =
//         writer.WriteStartArray()
//         name |> toList |> List.iter (fun part -> writer.WriteStringValue(part))
//         writer.WriteEndArray()

//     let inline writeJsonToStream (options: JsonWriterOptions) (stream: IO.Stream) (name: Name) =
//         use writer = new Utf8JsonWriter(stream, options)
//         writeJsonToWriter writer name

//     let toJsonWithOptions (name: Name) (options: MorphirJsonOptions) =
//         use stream = new IO.MemoryStream()
//         use writer = new Utf8JsonWriter(stream)
//         writeJsonToWriter writer name
//         writer.Flush()
//         Encoding.UTF8.GetString(stream.ToArray())

//     let inline toJson (name: Name) =
//         toJsonWithOptions name MorphirJsonOptions.Default

[<AutoOpen>]
module Json =

    let inline createWriterForStream
        (context: MorphirIRWriterContext)
        (stream: Stream)
        : 'Writer
              when 'Writer :> MorphirIRWriter
              and 'Writer: (static member CreateFromContext: MorphirIRWriterContext -> stream: Stream -> 'Writer)
        =
        'Writer.CreateFromContext(context, stream)

    let inline writeJsonWith
        (context: MorphirIRWriterContext)
        (writer: #MorphirIRWriter)
        (value: ^T when ^T: (static member WriteJsonTo: ^T * MorphirIRWriterContext * #MorphirIRWriter -> unit))
        =
        'T.WriteJsonTo(value, context, writer)

    let inline writeJsonToStream<'Writer, 'T
        when 'Writer :> MorphirIRWriter
        and 'Writer: (static member CreateFromContext: MorphirIRWriterContext -> stream: Stream -> 'Writer)
        and 'T: (static member WriteJsonTo: 'T * MorphirIRWriterContext * 'Writer -> unit)>
        (context: MorphirIRWriterContext)
        (stream: Stream)
        (value: 'T)
        =
        let writer = 'Writer.CreateFromContext(context, stream)
        'T.WriteJsonTo(value, context, writer)

    let inline toJson<'Writer, 'Value
        when 'Writer :> MorphirIRWriter
        and 'Writer: (static member CreateFromContext: MorphirIRWriterContext -> stream: Stream -> 'Writer)
        and 'Value: (static member WriteJsonTo: 'Value * MorphirIRWriterContext * 'Writer -> unit)>
        (context: MorphirIRWriterContext)
        (value: 'Value)
        =
        use ms = new MemoryStream()
        writeJsonToStream<'Writer, 'Value> context ms value
        let json = System.Text.Encoding.UTF8.GetString(ms.ToArray())
        json

    type MorphirIRWriterContext with

        static member Default: MorphirIRWriterContext =
            let props = ContextProperties()

            props.Set(ContextProperties.MorphirJsonOptions, MorphirJsonOptions.Default)
            |> ignore

            { Properties = props }

        member inline ctx.ConvertToJson<'Writer, 'Value
            when 'Writer :> MorphirIRWriter
            and 'Writer: (static member CreateFromContext: MorphirIRWriterContext -> stream: Stream -> 'Writer)
            and 'Value: (static member WriteJsonTo: 'Value * MorphirIRWriterContext * 'Writer -> unit)>
            (value: 'Value)
            : string =
            toJson<'Writer, 'Value> ctx value
