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

  let getFullPath relPath = Path.Combine (getRoot (), relPath)

open Internals

type Format = 
  | Html
  | Csv
  | Xml
  with
    static member FromString = function
      | "html" -> Format.Html
      | "csv" -> Format.Csv
      | "xml" -> Format.Xml
      | s -> invalidArg "format" (sprintf "invalid format parameter \"%s\"" s)
    override v.ToString () = (function Html -> "html" | Csv -> "csv" | Xml -> "xml") v

let setTemplatesDir dir =
  templatesDir <- Some dir

let renderTpl relPath data = renderPageFile (getFullPath relPath) data

let renderFmt (fmt: Format) tplName data = renderTpl (sprintf "%s.%s.liquid" tplName (fmt.ToString())) data

let commitInfo (fmt: Format) (data: CommitInfo) =
  renderFmt fmt "commitInfo" data

let main (fmt: Format) (data: Activities) = 
  data
  |> List.choose (function | Commit commit -> Some commit | _ -> None)
  |> List.map (commitInfo fmt >> Async.RunSynchronously)
  |> renderFmt fmt "commits"

