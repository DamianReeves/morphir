#r "nuget: Fun.Build, 1.0.5"

open Fun.Build
open Fun.Build.Internal

type PipelineBuilder with

  [<CustomOperation "collapseGithubActionLogs">]
  member inline this.collapseGithubActionLogs(build: Internal.BuildPipeline) =
    let build =
      this.runBeforeEachStage (
        build,
        (fun ctx ->
          if ctx.GetStageLevel() = 0 then
            printfn $"::group::{ctx.Name}")
      )

    this.runAfterEachStage (
      build,
      (fun ctx ->
        if ctx.GetStageLevel() = 0 then
          printfn "::endgroup::")
    )


let options =
  {| GithubAction = EnvArg.Create("GITHUB_ACTION", description = "Run only in in github action container")
     NugetAPIKey = EnvArg.Create("NUGET_API_KEY", description = "Nuget api key") |}


let stage_checkEnv =
  stage "Check environment" {
    run "dotnet tool restore"
    run (fun ctx -> printfn $"""github action name: {ctx.GetEnvVar options.GithubAction.Name}""")
  }

let stage_lint =
  stage "Lint" {
    stage "Format" {
      whenNot { envVar options.GithubAction }
      run "dotnet fantomas . -r"
    }

    stage "Check" {
      whenEnvVar options.GithubAction
      run "dotnet fantomas . -r --check"
    }
  }

let stage_test = stage "Run unit tests" { run "dotnet test " }

let stage_wasm_trimmed =
  stage "Build WASM Trimmed" {
    run "dotnet publish -f net8.0 -r wasi-wasm -c Release /p:MorphirAggressiveTrimming=true"
  }

pipeline "test" {
  description "Format code and run tests"

  stage_checkEnv
  stage_lint
  stage_test

  runIfOnlySpecified
}

pipeline "wasm" {
  description "Build WASM packages/components"

  stage_checkEnv
  stage_lint
  stage_test
  stage_wasm_trimmed

  runIfOnlySpecified
}

tryPrintPipelineCommandHelp ()
