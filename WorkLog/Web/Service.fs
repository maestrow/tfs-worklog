module Web.Service

open System
open System.Text.RegularExpressions
open Suave
open Suave.Successful
open Suave.Filters
open Suave.Operators
open Suave.Utils
open Suave.DotLiquid
open Utils.SuaveHelpers

open Logic.Commits
open Logic.TfsUrlTemplates
open Web.Renderers
open Web.ViewModels

open TestData

module D = Utils.Deserializer

Renderers.setTemplatesDir (__SOURCE_DIRECTORY__ + "/Views")

module private Internals =
  // ToDo: Unused function - the old one dates deserializer
  let parseDate str =
    let rx = Regex ("(?<year>\d\d\d\d)-(?<month>\d\d)-(?<day>\d\d)")
    let m = rx.Match(str)
    let ymd = ["year"; "month"; "day"] |> List.map (fun groupName -> m.Groups.Item(groupName).Value |> int)
    DateTime (ymd.[0], ymd.[1], ymd.[2])

  let renderError (errors: string list) = 
    errors |> String.concat "\n\n" |> OK

  let commitsAction repoParams commitsParams = 
    Logic.Commits.getCommits repoParams commitsParams
    |> List.map (fun ci -> Commit ci)
    |> Renderers.main
    |> Async.RunSynchronously
    |> OK

open Internals

let getParamsFromQuery (request: HttpRequest) = 
    let deserializerFn = D.deserializers |> dict |> D.fromString
    [typeof<RepoParams>; typeof<CommitsParams>] 
    |> D.getParamsFromQuery deserializerFn (request.query |> dict)
    |> function
        | Choice2Of2 errors -> errors |> renderError
        | Choice1Of2 objs -> 
          let repoParams = objs.[0] :?> RepoParams
          let commitsParams = objs.[1] :?> CommitsParams
          commitsAction repoParams commitsParams

let testAction1 () = Renderers.main (generateActivities())