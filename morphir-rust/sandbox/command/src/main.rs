mod bindings;

use bindings::morphir_ir::name::name;

fn main() {
    let nm = name::from_string("Hello");
    println!("Name: {:?}", nm);
}
