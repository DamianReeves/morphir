use clap::{Arg, Command};

pub fn cli() -> Command {
    let command = Command::new("morphir");
    let command = add_restore_subcommand(command);
    command
}

fn add_restore_subcommand(command: Command) -> Command {
    command.subcommand(
        Command::new("restore")
            .about("Restore the project dependencies")
            .arg(
                Arg::new("project_dir")
                    .short('d')
                    .long("project-dir")
                    .default_value("."),
            ),
    )
}
