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

open ViewModels
open Domain.UserInput
open TestData

setTemplatesDir (__SOURCE_DIRECTORY__ + "/Views")
Renderers.setTemplatesDir (__SOURCE_DIRECTORY__ + "/Views")

let render str = 
  fun ctx ->
    async {
      let! rendered = str
      return! OK rendered ctx
    }

let commitsAction (request: HttpRequest) = 
  request.query
  |> UserInput.FromParameters
  |> Logic.Commits.getCommits 
  |> List.map (fun ci -> Commit ci)
  |> Renderers.main
  |> Async.RunSynchronously
  |> OK

let app : WebPart =
  choose [ 
    path "/" >=> OK "Welcome!"
    path "/commits" >=> request commitsAction
    path "/test1" >=> render (Renderers.main (generateActivities()))
  ]


[<EntryPoint>]
let main argv =
  printfn "%A" argv
  startWebServer defaultConfig app
  0 // return an integer exit code
