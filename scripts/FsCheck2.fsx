#r "../packages/FsCheck/lib/net452/FsCheck.dll"

open System
open FsCheck
open FsCheck.Random


let createEvaluator gen = 
  let mutable seed = Random.newSeed()
  fun () ->
    seed <- Random.split seed |> snd
    Gen.eval 0 seed gen

let sample1 gn = Gen.sample 0 1 gn |> List.head

let threeIntsEval = 
  Gen.choose (1, 10)
  |> Gen.three
  |> createEvaluator


sample1 Arb.generate<Guid> |> printfn "%A"
sample1 Arb.generate<int> |> printfn "%A"
sample1 Arb.generate<int> |> printfn "%A"

let guidEval = 
  Arb.generate<Guid> |> createEvaluator

printfn "%A" (guidEval())
printfn "%A" (guidEval())