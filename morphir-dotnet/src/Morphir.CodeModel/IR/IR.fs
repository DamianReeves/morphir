namespace Morphir.IR

open Morphir.IR.Name
open Morphir.IR.Path

type IRDocumentNode<'TypeMetadata, 'ValueMetadata> =
    interface
    end

type IRNode<'T> =
    abstract member Attributes: 'T

module IRNode =
    let inline attributes<'T, 'A when 'T: (member Attributes: 'A)> (node: 'T) = node.Attributes
