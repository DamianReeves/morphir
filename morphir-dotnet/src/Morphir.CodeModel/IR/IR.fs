namespace Morphir.IR

open Morphir.IR.Name
open Morphir.IR.Path

type IRDocumentNode<'TypeMetadata, 'ValueMetadata> =
    interface end

type IRNode<'T> =
    abstract member Attributes: 'T

module IRNode =
    let inline attributes<'T, 'A when 'T: (member Attributes: 'A)> (node: 'T) = node.Attributes

type MorphirIRWriter =

    // abstract member WriteBooleanValue: bool -> unit
    abstract member WriteStringValue: MorphirIRWriterContext -> string -> unit
    // abstract member WriteNumberValue: int32 -> unit
    // abstract member WriteNumberValue: float32 -> unit

    abstract member WriteStartName: MorphirIRWriterContext -> unit
    abstract member WriteEndName: MorphirIRWriterContext -> unit
    
    abstract member WriteVariable: MorphirIRWriterContext -> Name -> unit
    
and MorphirIRWriter<'TypeMetadata, 'ValueMetadata>  =
    interface
    end
and MorphirIRWriterContext =
    { ActionHistory: MorphirIRWriterAction list }

    member c.CurrentAction: MorphirIRWriterAction option = c.ActionHistory |> List.tryHead

and MorphirIRWriterAction = | WritingName
