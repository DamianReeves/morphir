// Copyright 2024 FINOS
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//     http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
namespace Morphir.IR

open Morphir.Codecs.Json
open Morphir.IR.Codecs
open Morphir.IR.Codecs.Json

module Name =
    open Morphir.SDK.Maybe
    open Morphir.SDK
    open Morphir.Codecs
    open Morphir.CodeModel

    [<Struct>]
    type Name =
        | Name of Parts: string list

        static member inline FromList(words: string list) : Name = Name words
        static member inline ToList(Name parts) = parts

        static member ToTitleCase name =
            name |> Name.ToList |> List.map String.capitalize |> String.join ""

        static member ToCamelCase(name: Name) =
            match name |> Name.ToList with
            | [] -> System.String.Empty
            | head :: tail -> tail |> List.map String.capitalize |> List.cons head |> String.join ""

        static member ToHumanWords name : List<string> =
            let words = Name.ToList name

            let join abbrev =
                abbrev |> String.join "" |> String.toUpper

            let rec process' prefix abbrev suffix =
                match suffix with
                | [] ->
                    if (List.isEmpty abbrev) then
                        prefix
                    else
                        List.append prefix [ join abbrev ]
                | first :: rest ->
                    if (String.length first = 1) then
                        process' prefix (List.append abbrev [ first ]) rest
                    else
                        match abbrev with
                        | [] -> process' (List.append prefix [ first ]) [] rest
                        | _ -> process' (List.append prefix [ join abbrev; first ]) [] rest

            process' [] [] words

        static member ToSnakeCase name =
            name |> Name.ToHumanWords |> String.join "_"

        static member ToKebabCase name =
            name |> Name.ToHumanWords |> String.join "-"

        static member WriteJsonTo(name: Name, writer: MorphirJsonWriter) =
            match writer.JsonOptions.NameEncodingMode with
            | NameEncodingMode.AsArray ->
                writer.StartContainer(MorphirContainerType.List)
                name |> Name.ToList |> List.iter (fun part -> writer.WriteStringValue part)
                writer.EndContainer()
            | NameEncodingMode.AsIdentifier ->
                let formattedName = name |> Name.ToKebabCase
                writer.WriteStringValue $"_:{formattedName}"
            | NameEncodingMode.AsIri -> failwith "Not implemented"

        static member PartsFromString(input: string) : string list =
            let wordPattern =
                Regex.fromString "([a-zA-Z][a-z]*|[0-9]+)" |> Maybe.withDefault Regex.never

            Regex.find wordPattern input
            |> List.map (fun me -> me.Match)
            |> List.map String.toLower


    let inline fromList (words: string list) : Name = Name words

    let inline partsFromString input = Name.PartsFromString input

    let fromString (input: string) : Name =
        input |> Name.PartsFromString |> fromList

    let inline toList (Name name) : List<string> = name

    let capitalize string : string =
        match String.uncons string with
        | Just(headChar, tailString) -> String.cons (Char.toUpper headChar) tailString
        | Nothing -> string

    let inline toTitleCase (name: Name) = Name.ToTitleCase name
    let inline toCamelCase (name: Name) = Name.ToCamelCase name
    let inline toHumanWords (name: Name) : List<string> = Name.ToHumanWords name
    let inline toSnakeCase (name: Name) : string = Name.ToSnakeCase name
    let inline toKebabCase (name: Name) : string = Name.ToKebabCase name

open Name

type NameBuilder() =
    member inline _.Combine(Name existing, Name other) : Name = existing @ other |> Name.fromList
    member inline _.Delay(f: _ -> Name) = f ()
    member inline _.Return(nameStr: string) = Name.fromString nameStr
    member inline _.ReturnFrom(name: Name) = name

    member inline _.Yield(input: string) = Name.fromString input
