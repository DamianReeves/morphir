namespace Morphir.CodeModel

type MorphirCoreType =
    | None = 0x00
    | Bool = 0x01
    | Decimal = 0x02
    | Float = 0x03
    | Int = 0x04
    | List = 0x05
    | String = 0x06
    | Record = 0x07
    | TaggedUnion = 0x08
and MorphirContainerType =
    | List
    | Map
    | Record
    | Tuple
    | TaggedUnion

module MorphirCoreType =
    let IsScalar (cType:MorphirCoreType)  =
        match cType with
        | MorphirCoreType.Bool | MorphirCoreType.Bool | MorphirCoreType.Bool | MorphirCoreType.Bool | MorphirCoreType.Bool -> true
        | _ -> false