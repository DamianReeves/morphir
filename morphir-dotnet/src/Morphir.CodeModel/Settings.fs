namespace Morphir.Model

open System
open System.Collections.Concurrent

type ISetting =
    abstract member Name: string
    abstract member IsRequired: bool
    abstract member HasDefault: bool

and Setting<'Value> =
    | Required of Name: string * Default: 'Value
    | Optional of Name: string * Default: 'Value option

    member this.HasDefault =
        match this with
        | Required _ -> true
        | Optional(_, defaultValue) -> defaultValue.IsSome

    member this.Name =
        match this with
        | Required(name, _) -> name
        | Optional(name, _) -> name

    interface ISetting with
        member this.HasDefault = this.HasDefault

        member this.IsRequired =
            match this with
            | Required _ -> true
            | Optional _ -> false

        member this.Name = this.Name


and Settings private (underlyingValues: ConcurrentDictionary<ISetting, obj>) =
    new() = Settings(ConcurrentDictionary<ISetting, obj>())

    member inline private this.GetCore(setting: Setting<'Value>) : 'Value option =
        match underlyingValues.TryGetValue(setting) with
        | true, (:? 'Value as value) -> value |> Some
        | _ -> None

    member this.Get(setting: Setting<'Value>) : Option<'Value> =
        match this.GetCore(setting) with
        | None when setting.HasDefault ->
            match setting with
            | Required(_, defaultValue) -> defaultValue |> Some
            | Optional(_, defaultValue) -> defaultValue
        | res -> res

    member this.GetOrDefault(setting: Setting<'Value>, defaultValue: 'Value) : 'Value =
        this.GetCore setting |> Option.defaultValue (defaultValue)

    member this.GetOrDefault(setting: Setting<'Value>, defaultValueProvider: unit -> 'Value) : 'Value =
        this.GetCore setting |> Option.defaultValue (defaultValueProvider ())

    member _.Set(setting: Setting<'Value>, value: 'Value) =
        let valueAsObj = value :> obj

        underlyingValues.AddOrUpdate(setting, valueAsObj, (fun _ _ -> valueAsObj))
        |> ignore
