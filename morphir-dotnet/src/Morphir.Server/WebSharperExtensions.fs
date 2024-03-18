namespace WebSharper.Core.Json
    
namespace WebSharper.Sitelets 
    open WebSharper.Sitelets


    type CustomEncoder<'T> = 'T -> J.Encoded
    [<AutoOpen>]
    module ContentExtensions =
        module J = WebSharper.Core.Json
        
        
        
        type Content<'Endpoint> with
            member c.MorphirJson(encoder:J.Encoder, value:'T) =
                let encoded = encoder.Encode value
                Content.Json(encoded)

    module Content =
        module J = WebSharper.Core.Json 
        let morphirJson (encoder:J.Encoder) (value:'T) =
            let encoded = encoder.Encode value
            Content.Json(encoded)
        