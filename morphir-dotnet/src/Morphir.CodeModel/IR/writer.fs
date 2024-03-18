namespace Morphir.IR.Codecs

open Morphir.IR.Name

type MorphirIRWriter =

    // abstract member WriteBooleanValue: bool -> unit
    abstract member WriteStringValue: context: MorphirIRWriterContext -> value: string -> unit
    // abstract member WriteNumberValue: int32 -> unit
    // abstract member WriteNumberValue: float32 -> unit

    abstract member WriteStartName: context: MorphirIRWriterContext -> unit
    abstract member WriteEndName: context: MorphirIRWriterContext -> unit

    abstract member WriteVariable: MorphirIRWriterContext -> Name -> unit

and MorphirIRWriter<'TypeMetadata, 'ValueMetadata> =
    interface
    end

//    member c.CurrentAction: MorphirIRWriterAction option = c.ActionHistory |> List.tryHead

and MorphirIRWriterAction = | WritingName
