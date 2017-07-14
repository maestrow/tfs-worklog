module WorkLog

open System
open System.Text.RegularExpressions
open FSharp.Core
open Suave
open Suave.Successful
open Suave.Filters
open Suave.Operators
open Suave.Utils
open Suave.DotLiquid
open DotLiquid

open Web.Service

setTemplatesDir (__SOURCE_DIRECTORY__ + "/Views")

let render str = 
  fun ctx ->
    async {
      let! rendered = str
      return! OK rendered ctx
    }

let app : WebPart =
  choose [ 
    path "/" >=> OK "Welcome!"
    path "/commits" >=> request getParamsFromQuery
    path "/test1" >=> render (testAction1())
  ]


[<EntryPoint>]
let main argv =
  printfn "%A" argv
  startWebServer defaultConfig app
  0 // return an integer exit code
