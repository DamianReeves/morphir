namespace Morphir.Server

open WebSharper
open WebSharper.Sitelets
open WebSharper.UI
open WebSharper.UI.Server

type EndPoint =
    | [<EndPoint "/">] Home
    | [<EndPoint "/about">] About
    | [<EndPoint "/sandbox/encode/name">] EncodeName of string
    | [<EndPoint "/api/sandbox/websharper-json">] WebSharperJson of string 

module Templating =
    open WebSharper.UI.Html

    // Compute a menubar where the menu item for the given endpoint is active
    let MenuBar (ctx: Context<EndPoint>) endpoint : Doc list =
        let (=>) txt act =
            let isActive = if endpoint = act then "nav-link active" else "nav-link"
            li [ attr.``class`` "nav-item" ] [ a [ attr.``class`` isActive; attr.href (ctx.Link act) ] [ text txt ] ]

        [ "Home" => EndPoint.Home; "About" => EndPoint.About ]

    let Main ctx action (title: string) (body: Doc list) =
        Content.Page(
            Templates
                .MainTemplate()
                .Title(title)
                .MenuBar(MenuBar ctx action)
                .Body(body)
                .Doc()
        )

module Site =
    open WebSharper.UI.Html

    open type WebSharper.UI.ClientServer
    open Morphir.IR
    open Morphir.IR.Codec
    module J = WebSharper.Core.Json

    let HomePage ctx =
        Templating.Main
            ctx
            EndPoint.Home
            "Home"
            [ h1 [] [ text "Say Hi to the server!" ]; div [] [ client (Client.Main()) ] ]

    let AboutPage ctx =
        Templating.Main
            ctx
            EndPoint.About
            "About"
            [ h1 [] [ text "About" ]
              p [] [ text "This is a template WebSharper client-server application." ] ]

    let EncodeNameApi ctx nameStr =
        let name = nameStr |> Name.fromString
        let encodedName = name |> Name.toJson

        Content.Custom(
            Status = Http.Status.Ok,
            Headers = [ Http.Header.Custom "Content-Type" "application/json" ],
            WriteBody =
                fun stream ->
                    let json = Name.toJson name
                    let bytes = System.Text.Encoding.UTF8.GetBytes(json)
                    stream.Write(bytes, 0, bytes.Length)
        )

    [<Website>]
    let Main =
        let encoder =  { new J.Encoder<string> with
            member Encode : json =
                J.Encoded.Array [ J.Encoded.Lift (J.Value.String json)]
        }
        
        Application.MultiPage(fun ctx endpoint ->
            match endpoint with
            | EndPoint.Home -> HomePage ctx
            | EndPoint.About -> AboutPage ctx
            | EndPoint.EncodeName name -> EncodeNameApi ctx name
            | EndPoint.WebSharperJson json ->
                Content.morphirJson encoder json
        )
