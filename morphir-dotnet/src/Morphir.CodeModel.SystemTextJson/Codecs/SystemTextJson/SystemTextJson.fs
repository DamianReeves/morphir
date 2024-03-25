namespace Morphir.Codecs.SystemTextJson

open Morphir.Codecs.Json
open Morphir.Codecs
open Morphir.CodeModel
open System
open System.Collections.Concurrent
open System.Buffers
open System.IO
open System.Text.Json

type MorphirUtf8JsonWriter(options: MorphirJsonFormatOptions, streamOrWriter: Choice<Stream, IBufferWriter<Byte>>) =
    inherit MorphirJsonWriter(options)

    let stack = new ConcurrentStack<State>()

    let writer =
        match streamOrWriter with
        | Choice1Of2(stream) -> new Utf8JsonWriter(stream)
        | Choice2Of2(writer) -> new Utf8JsonWriter(writer)

    new(options: MorphirJsonFormatOptions, stream: Stream) = new MorphirUtf8JsonWriter(options, Choice1Of2(stream))

    new(options: MorphirJsonFormatOptions, writer: IBufferWriter<Byte>) =
        new MorphirUtf8JsonWriter(options, Choice2Of2(writer))

    override this.Dispose() = writer.Dispose()

    override this.EndContainer() =
        match stack.TryPop() with
        | true, InContainer MorphirContainerType.List -> writer.WriteEndArray()
        | _ -> ()

    override this.StartContainer(containerType: Morphir.CodeModel.MorphirContainerType) =
        stack.Push(InContainer containerType)
        writer.WriteStartArray()

    override this.WriteBooleanValue(value: bool) = ()
    override this.WriteDecimalValue(value: decimal) = ()
    override this.WriteDoubleValue(value: double) = ()

    override this.WriteFloat32Value(value: float32) = ()
    override this.WriteInt8Value(value: int8) = ()
    override this.WriteInt16Value(value: int16) = ()
    override this.WriteInt32Value(value: int32) = ()
    override this.WriteInt64Value(value: int64) = ()
    override this.WriteStringValue(value: string) = writer.WriteStringValue(value)

and State = InContainer of Morphir.CodeModel.MorphirContainerType
