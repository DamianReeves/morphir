import { desc, run, task } from "https://deno.land/x/drake@v1.6.0/mod.ts";
import { clone as gitClone} from "./scripts/deno/git-clone.ts";

desc("Minimal Drake task");
task("hello", [], function () {
  console.log("Hello World!");
});

desc("Clone a git repository");
task("clone", [], async function () {
  await gitClone("https://github.com/finos/morphir-elm.git", "morphir-elm");
});

run();