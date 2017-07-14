module Web.Renderers

open System.IO
open DotLiquid
open Suave.DotLiquid
open ViewModels

module private Internals = 

  let mutable templatesDir : string option = None
  
  let getRoot () =
    if templatesDir.IsNone then failwith "The templates folder is not specified. Specify one with setTemplatesDir."
    templatesDir.Value

open Internals

let setTemplatesDir dir =
  templatesDir <- Some dir


let commitInfo (data: CommitInfo) =
  renderPageFile (Path.Combine (getRoot (), "commitInfo.liquid")) data

let main (data: Activities) = 
  data
  |> List.choose (function | Commit commit -> Some commit | _ -> None)
  |> List.map (commitInfo >> Async.RunSynchronously)
  |> renderPageFile (Path.Combine (getRoot (), "index.liquid"))

