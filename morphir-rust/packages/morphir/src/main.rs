mod cli;
use cli::cli;

fn main() {
    let matches = cli().get_matches();
    println!("{:?}", matches);
}
