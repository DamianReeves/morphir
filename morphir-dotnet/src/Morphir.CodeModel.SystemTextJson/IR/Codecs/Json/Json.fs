namespace Morphir.IR.Codecs.Json

open System
open System.Buffers
open System.IO
open System.Text
open System.Text.Json
open System.Text.Json.Serialization
open Morphir.IR
open Morphir.IR.Codecs


type MorphirJsonIRWriter private (options: MorphirJsonOptions, streamOrWriter: Choice<Stream, IBufferWriter<Byte>>) =
    let writer =
        match streamOrWriter with
        | Choice1Of2(stream) -> new Utf8JsonWriter(stream)
        | Choice2Of2(writer) -> new Utf8JsonWriter(writer)

    let sb = new StringBuilder()

    new(options: MorphirJsonOptions, stream: Stream) = MorphirJsonIRWriter(options, Choice1Of2(stream))
    new(options: MorphirJsonOptions, writer: IBufferWriter<Byte>) = MorphirJsonIRWriter(options, Choice2Of2(writer))

    static member CreateFromContext(context:MorphirIRWriterContext, stream:Stream): MorphirJsonIRWriter =
        let options = context.Properties.GetOrDefault<MorphirJsonOptions>(ContextProperties.MorphirJsonOptions, MorphirJsonOptions.Default)
        new MorphirJsonIRWriter(options, stream)

    member this.WriteStringValue(value: string) = failwith "todo"

    member this.WriteStartName() =
        if options.WriteNamesAsArrays then
            writer.WriteStartArray()
        else
            sb.Clear() |> ignore

    member this.WriteEndName() =
        if options.WriteNamesAsArrays then
            writer.WriteEndArray()
        else
            writer.WriteStringValue(sb.ToString())

    interface MorphirIRWriter with
        member this.WriteEndName(context) = this.WriteEndName()
        member this.WriteStartName(context) = this.WriteStartName()
        member this.WriteStringValue context value = this.WriteStringValue(value)

        member this.WriteVariable (arg1: MorphirIRWriterContext) (arg2: Morphir.IR.Name.Name) : unit =
            failwith "Not Implemented"

module Name =
    open Morphir.IR.Name

    let private DefaultOptions = JsonFSharpOptions.Default

    let writeJsonToWriter (writer: Utf8JsonWriter) (name: Name) =
        writer.WriteStartArray()
        name |> toList |> List.iter (fun part -> writer.WriteStringValue(part))
        writer.WriteEndArray()

    let inline writeJsonToStream (options: JsonWriterOptions) (stream: IO.Stream) (name: Name) =
        use writer = new Utf8JsonWriter(stream, options)
        writeJsonToWriter writer name

    let toJsonWithOptions (name: Name) (options: MorphirJsonOptions) =
        use stream = new IO.MemoryStream()
        use writer = new Utf8JsonWriter(stream)
        writeJsonToWriter writer name
        writer.Flush()
        Encoding.UTF8.GetString(stream.ToArray())

    let inline toJson (name: Name) =
        toJsonWithOptions name MorphirJsonOptions.Default


// let writeJsonToStreamAsync
//     (f: 'a -> Utf8JsonWriter -> Async<unit>)
//     (options: JsonWriterOptions)
//     (stream: IO.Stream)
//     =
//     use writer = new Utf8JsonWriter(stream, options)
//     f value writer
