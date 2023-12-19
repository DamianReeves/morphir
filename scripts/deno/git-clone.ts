
interface Options {
  git?: string;
  shallow?: boolean;
  progress?: boolean;
  args?: string[];
}


export function clone(repo:string, targetPath:string, options:Options = {}) {
  const [cmd, args] = buildCloneCommand(repo, targetPath, options);
  new Promise(resolve => {
    console.log("git-clone:", cmd, args);
    resolve(true);
  })
}

function buildCloneCommand(repo:string, targetPath:string, options:Options) {
  let args = ["clone"];
  const userArgs = options.args || [];
  if(options.shallow) {
    if(userArgs.includes("--depth")) {
      throw new Error("'--depth' cannot be specified when shallow is set to 'true'")
    }
    args.push("--depth", "1");
  }

  if(options.progress) {
    args.push("--progress");
  }

  args = args.concat(userArgs);
  args.push("--", repo, targetPath);

  return [git(options), args]
}

function git(options:Options) {
  return options.git || "git";
}