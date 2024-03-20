namespace Morphir.Codecs

open System
open System.Runtime.CompilerServices
open Morphir.CodeModel

type IMorphirWriter =
    inherit IDisposable
    abstract member Options: MorphirFormatOptions
    abstract member WriteBooleanValue: value: bool -> unit
    abstract member WriteDecimalValue: value: decimal -> unit
    abstract member WriteDoubleValue: value: double -> unit
    abstract member WriteFloat32Value: value: float32 -> unit
    abstract member WriteInt8Value: value: int8 -> unit
    abstract member WriteInt16Value: value: int16 -> unit
    abstract member WriteInt32Value: value: int32 -> unit
    abstract member WriteInt64Value: value: int64 -> unit
    abstract member WriteStringValue: value: string -> unit
    abstract member StartContainer: containerType: MorphirContainerType -> unit
    abstract member EndContainer: unit -> unit

and [<AbstractClass>] MorphirWriter(options: MorphirFormatOptions) =
    member this.Options = options
    abstract member WriteBooleanValue: value: bool -> unit
    abstract member WriteDecimalValue: value: decimal -> unit
    abstract member WriteDoubleValue: value: double -> unit
    abstract member WriteFloat32Value: value: float32 -> unit
    abstract member WriteInt8Value: value: int8 -> unit
    abstract member WriteInt16Value: value: int16 -> unit
    abstract member WriteInt32Value: value: int32 -> unit
    abstract member WriteInt64Value: value: int64 -> unit
    abstract member WriteStringValue: value: string -> unit
    abstract member StartContainer: containerType: MorphirContainerType -> unit
    abstract member EndContainer: unit -> unit
    abstract member Dispose: unit -> unit

    interface IMorphirWriter with
        member this.Options = this.Options
        member this.Dispose() : unit = this.Dispose()
        member this.EndContainer() : unit = this.EndContainer()
        member this.StartContainer(containerType: MorphirContainerType) : unit = this.StartContainer(containerType)
        member this.WriteBooleanValue(value: bool) : unit = this.WriteBooleanValue(value)
        member this.WriteDecimalValue(value: decimal) : unit = this.WriteDecimalValue(value)
        member this.WriteDoubleValue(value: double) : unit = this.WriteDoubleValue(value)
        member this.WriteFloat32Value(value: float32) : unit = this.WriteFloat32Value(value)
        member this.WriteInt16Value(value: int16) : unit = this.WriteInt16Value(value)
        member this.WriteInt32Value(value: int32) : unit = this.WriteInt32Value(value)
        member this.WriteInt64Value(value: int64) : unit = this.WriteInt64Value(value)
        member this.WriteInt8Value(value: int8) : unit = this.WriteInt8Value(value)
        member this.WriteStringValue(value: string) : unit = this.WriteStringValue(value)


and MorphirWriteAction =
    | SetFormatOptions of Options: MorphirFormatOptions
    | WriteBooleanValue of Value: bool
    | WriteDecimalValue of Value: decimal
    | WriteDoubleValue of Value: double
    | WriteFloat32Value of Value: float32
    | WriteInt8Value of Value: int8
    | WriteInt16Value of Value: int16
    | WriteInt32Value of Value: int32
    | WriteInt64Value of Value: int64
    | WriteStringValue of Value: string
    | StartContainer of MorphirContainerType
    | EndContainer

[<AutoOpen>]
module MorphirWriterExtensions =
    type IMorphirWriter with

        member inline w.WriteByteValue(value: byte) = w.WriteInt8Value(int8 value)

[<Extension>]
type MorphirWriterExtensions =
    [<Extension>]
    static member WriteByteValue(w: IMorphirWriter, value: byte) = w.WriteInt8Value(int8 value)
