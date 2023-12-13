open System.Runtime.InteropServices

[<EntryPoint>]
let main argv =
  printfn "Hello: %s:%A" (RuntimeInformation.OSDescription) (RuntimeInformation.OSArchitecture)
  printfn "With love from %s" (RuntimeInformation.FrameworkDescription)
  printf "%A" argv
  0
