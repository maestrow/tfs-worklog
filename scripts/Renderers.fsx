#r "../packages/DotLiquid/lib/net451/DotLiquid.dll"
#r "../packages/Suave/lib/net40/Suave.dll"
#r "../packages/Suave.DotLiquid/lib/net40/Suave.DotLiquid.dll"

#load "../WorkLog/Extensions.fs"
#load "../WorkLog/Utils/TestDataGenerators.fs"
#load "../WorkLog/ViewModels/ViewModels.fs"
#load "../WorkLog/TestData.fs"
#load "../WorkLog/ViewModels/Renderers.fs"

open System
open System.IO
open TestData
open Renderers
open ViewModels
open Suave.Successful
open Suave.DotLiquid

Renderers.setTemplatesDir (__SOURCE_DIRECTORY__ + "/Views")

let render renderer = 
  fun ctx ->
    async {
      let! rendered = renderer
      return! OK rendered ctx
    }

let commitInfo (data: CommitInfo) =
  renderPageFile (Path.Combine (getRoot (), "commitInfo.liquid")) data

let data = generateActivities()

let activity = data |> List.head 

let commit = activity |> (fun (Commit commit) -> commit) 

let result = 
  renderPageFile (Path.Combine (getRoot (), "commitInfo.liquid")) commit

//let result = Renderers.main data