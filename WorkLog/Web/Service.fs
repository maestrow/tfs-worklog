module Web.Service

open System
open System.Text.RegularExpressions
open Suave
open Suave.Successful
open Suave.Writers
open Suave.Filters
open Suave.Operators
open Suave.Utils
open Suave.DotLiquid
open Utils.SuaveHelpers

open Logic.Commits
open Logic.TfsUrlTemplates
open Web.ViewModels

open TestData

module D = Utils.Deserializer

module private Internals =
  // ToDo: Unused function - the old one dates deserializer
  let parseDate str =
    let rx = Regex ("(?<year>\d\d\d\d)-(?<month>\d\d)-(?<day>\d\d)")
    let m = rx.Match(str)
    let ymd = ["year"; "month"; "day"] |> List.map (fun groupName -> m.Groups.Item(groupName).Value |> int)
    DateTime (ymd.[0], ymd.[1], ymd.[2])

  let renderError (errors: string list) = 
    errors |> String.concat "\n\n" |> OK

  let commitsAction repoParams commitsParams format = 
    let mime = 
      match format with
      | Renderers.Format.Html -> "text/html"
      | Renderers.Csv -> "application/zip"
      |> setMimeType
    Logic.Commits.getCommits repoParams commitsParams
    |> List.map (fun ci -> Commit ci)
    |> Renderers.main format
    |> Async.RunSynchronously
    |> (fun s -> mime >=> OK s)

open Internals

let getParamsFromQuery (request: HttpRequest) = 
    let deserializerFn = D.deserializers |> dict |> D.fromString
    [typeof<RepoParams>; typeof<CommitsParams>] 
    |> D.getParamsFromQuery deserializerFn (request.query |> dict)
    |> function
        | Choice2Of2 errors -> errors |> renderError
        | Choice1Of2 objs -> 
          let format = request.query ^^ "format" |> Choice.orDefault "html" |> Renderers.Format.FromString
          let repoParams = objs.[0] :?> RepoParams
          let commitsParams = objs.[1] :?> CommitsParams
          commitsAction repoParams commitsParams format

let testAction1 () = Renderers.main Renderers.Format.Html (generateActivities())