mod bindings;

use crate::bindings::exports::morphir_ir::name::name::{Guest, Name, Run};

struct Component;

impl Guest for Component {
    fn from_string(input: String) -> Name {
        println!("from_string: {}", input);
        let run = Run::Run(input);
        let res = Name::Name(vec![run]);
        res
    }

    fn to_string(name: Name) -> String {
        println!("to_string: {:?}", name);
        todo!("Not done yet")
    }
}

bindings::export!(Component with_types_in bindings);
