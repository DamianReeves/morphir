module Morphir.Visual.ViewReference exposing (..)

import Element exposing (Element, padding, spacing, text)
import Element.Events exposing (onClick)
import Morphir.IR.FQName exposing (FQName)
import Morphir.IR.Type exposing (Type)
import Morphir.IR.Value exposing (Value)
import Morphir.Visual.Common exposing (nameToText)
import Morphir.Visual.Config exposing (Config)


view : Config msg -> (Value ta (Type ta) -> Element msg) -> FQName -> Element msg
view config viewValue (( packageName, moduleName, localName ) as fQName) =
    Element.row
        [ padding config.state.theme.smallPadding
        , spacing config.state.theme.smallSpacing
        , onClick (config.handlers.onReferenceClicked fQName False)
        ]
        [ Element.el []
            (text
                (nameToText localName)
            )
        ]
