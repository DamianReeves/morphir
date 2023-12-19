
enum Source {
  CodeBlock(path: String, code: String, start_pos: Option<Position>, end_pos: Option<Position>),
  SourceFile(path: String, code: String),
}

struct Position {
  line: u32,
  column: u32,
}