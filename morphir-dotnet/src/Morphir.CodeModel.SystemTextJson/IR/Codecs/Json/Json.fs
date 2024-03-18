namespace Morphir.IR.Codecs.Json

open System
open System.Buffers
open System.IO
open System.Text
open System.Text.Json
open System.Text.Json.Serialization
open Morphir.IR

type MorphirJsonFormatVersion =
    | V3
    | V5

    static member Default = V3

type MorphirJsonOptions =
    { FormatVersion: MorphirJsonFormatVersion }

    static member Default = { FormatVersion = MorphirJsonFormatVersion.Default }

    member o.WriteNamesAsArrays =
        match o.FormatVersion with
        | MorphirJsonFormatVersion.V3 -> true
        | MorphirJsonFormatVersion.V5 -> false

    member o.WriteNamesAsStrings = not o.WriteNamesAsArrays

type MorphirJsonIRWriter private (options: MorphirJsonOptions, streamOrWriter: Choice<Stream, IBufferWriter<Byte>>) =
    let writer =
        match streamOrWriter with
        | Choice1Of2(stream) -> new Utf8JsonWriter(stream)
        | Choice2Of2(writer) -> new Utf8JsonWriter(writer)

    let sb = new StringBuilder()        

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
        member this.WriteEndName() = this.WriteEndName()
        member this.WriteStartName() = this.WriteStartName()
        member this.WriteStringValue(value) = this.WriteStringValue(value)

module Name =
    open Morphir.IR.Name

    let writeJsonToWriter (writer: Utf8JsonWriter) (name: Name) =
        writer.WriteStartArray()
        name |> toList |> List.iter (fun part -> writer.WriteStringValue(part))
        writer.WriteEndArray()

    let inline writeJsonToStream (options: JsonWriterOptions) (stream: IO.Stream) (name: Name) =
        use writer = new Utf8JsonWriter(stream, options)
        writeJsonToWriter writer name

    let toJsonWithOptions (name: Name) (options: JsonFSharpOptions) =
        use stream = new IO.MemoryStream()
        use writer = new Utf8JsonWriter(stream)
        writeJsonToWriter writer name
        writer.Flush()
        Encoding.UTF8.GetString(stream.ToArray())

    let inline toJson (name: Name) =
        toJsonWithOptions name Json.DefaultJsonOptions


// let writeJsonToStreamAsync
//     (f: 'a -> Utf8JsonWriter -> Async<unit>)
//     (options: JsonWriterOptions)
//     (stream: IO.Stream)
//     =
    //     use writer = new Utf8JsonWriter(stream, options)
//     f value writer
