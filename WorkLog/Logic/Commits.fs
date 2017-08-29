module Logic.Commits

open System
open System.IO
open System.Net
open System.Reflection
open System.Text.RegularExpressions

//open Suave
open FSharp.Data

open Utils.Reflection
open Domain.Settings
open Domain.Tfs
open TfsUrlTemplates
open Logic.TfsRequest
open Web.ViewModels


type RepoParams = {
  collection: string
  project: string
  repoId: string 
}

module private Implementation = 

  let commitDetailsCache = Utils.StringCache.initCache "cache"

  let getUrlTpl tplBldr (repoParams: RepoParams) = 
    tplBldr 
      settings.ServiceUrl.Schema 
      settings.ServiceUrl.Host
      repoParams.collection
      repoParams.project
      repoParams.repoId

  let getIssueIdFromComment comment =
    let rx = Regex (@"\n\nRelated Work Items: \#(\d+)$")
    let m = rx.Match comment
    match m.Success with
    | true -> m.Groups.[1].Value |> int
    | false -> -1

  let formatMessage s = 
    let rx = Regex "\n+"    
    rx.Replace(s, " ")

  let getCommitInfoFromShortJsonObj (repoParams: RepoParams) (jsonObj: TfsCommits.Value) =
    let issueId = getIssueIdFromComment jsonObj.Comment
    { 
      CommitInfo.id = jsonObj.CommitId 
      date = jsonObj.Committer.Date 
      issueId = issueId 
      message = formatMessage jsonObj.Comment 

      schema = settings.ServiceUrl.Schema
      host = settings.ServiceUrl.Host
      collection = repoParams.collection
      project = repoParams.project
      repoId = repoParams.repoId
    }

  // ToDo: set constraint and remove duplicate function definition
  let getCommitInfoFromDetailedJsonObj (repoParams: RepoParams) (jsonObj: TfsCommitDetails.Root) = 
    let issueId = getIssueIdFromComment jsonObj.Comment
    { 
      CommitInfo.id = jsonObj.CommitId 
      date = jsonObj.Committer.Date 
      issueId = issueId 
      message = formatMessage jsonObj.Comment 

      schema = settings.ServiceUrl.Schema
      host = settings.ServiceUrl.Host
      collection = repoParams.collection
      project = repoParams.project
      repoId = repoParams.repoId
    }

  let rxTrimMsg = Regex "Related Work Items: #\d+$" 

  let trimMessage (msg: string) = rxTrimMsg.Replace(msg, String.Empty)

  let postProcess (ci: CommitInfo) : CommitInfo = { ci with message = trimMessage ci.message }
   
open Implementation

let getCommits (repoParams: RepoParams) (commitsParams: CommitsParams) =
  let getCommitDetails commitId = 
    commitId
    |> getUrlTpl Tpl.commitDetails repoParams
    |> request
  
  let commits = 
    commitsParams
    |> getUrlTpl Tpl.commits repoParams
    |> request
    |> TfsCommits.Parse
    |> fun root -> root.Value
    |> List.ofArray
    |> List.groupBy (fun c -> c.CommentTruncated.IsSome && c.CommentTruncated.Value)
    |> dict

  let notTruncated = 
    commits.[false]
    |> List.map (getCommitInfoFromShortJsonObj repoParams)
  
  let truncated = 
    
    let mapFn = 
      fun (c: TfsCommits.Value) -> c.CommitId
      >> commitDetailsCache getCommitDetails
      >> TfsCommitDetails.Parse
      >> getCommitInfoFromDetailedJsonObj repoParams
    commits.[true] |> List.map mapFn

  [notTruncated; truncated] 
  |> List.concat
  |> List.map postProcess
  |> List.sortBy (fun c -> c.date)
