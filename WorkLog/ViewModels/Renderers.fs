module Renderers

open System.IO
open DotLiquid
open Suave.DotLiquid
open ViewModels

let mutable private templatesDir : string option = None

let setTemplatesDir dir =
  templatesDir <- Some dir

let getRoot () =
  if templatesDir.IsNone then failwith "The templates folder is not specified. Specify one with setTemplatesDir."
  templatesDir.Value

let commitInfo (data: CommitInfo) =
  renderPageFile (Path.Combine (getRoot (), "commitInfo.liquid")) data

let main (data: Activities) = 
  let chooser = function 
    | Commit commit -> Some commit 
    | _ -> None
  let renderer = renderPageFile (Path.Combine (getRoot (), "index2.liquid"))
  let model = 
    data
    |> List.choose chooser
    |> List.map (commitInfo >> Async.RunSynchronously)
  model |> renderer

