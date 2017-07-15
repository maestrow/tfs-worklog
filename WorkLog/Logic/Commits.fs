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
open Logic.TfsUrlTemplates
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
    

  let getCommitInfoFromShortJsonObj (jsonObj: TfsCommits.Value) =
    let issueId = getIssueIdFromComment jsonObj.Comment
    { 
      CommitInfo.id = jsonObj.CommitId; 
      date = jsonObj.Committer.Date; 
      issueId = issueId; 
      message = jsonObj.Comment; 
      url = jsonObj.Url 
    }

  // ToDo: set constraint and remove duplicate function definition
  let getCommitInfoFromDetailedJsonObj (jsonObj: TfsCommitDetails.Root) = 
    let issueId = getIssueIdFromComment jsonObj.Comment
    { 
      CommitInfo.id = jsonObj.CommitId; 
      date = jsonObj.Committer.Date; 
      issueId = issueId; 
      message = jsonObj.Comment; 
      url = jsonObj.Url 
    }
   
open Implementation

let getCommits (repoParams: RepoParams) (commitsParams: CommitsParams) =
  let getCommitDetails commitId = 
    commitId
    |> getUrlTpl Tpl.commitDetails repoParams
    |> request
  
  let commits = 
    commitsParams
    |> (getUrlTpl Tpl.commits repoParams)
    |> request
    |> TfsCommits.Parse
    |> (fun root -> root.Value)
    |> List.ofArray
    |> List.groupBy (fun c -> c.CommentTruncated.IsSome && c.CommentTruncated.Value)
    |> dict

  let notTruncated = 
    commits.[false]
    |> List.map getCommitInfoFromShortJsonObj
  
  let truncated = 
    
    let mapFn = 
      (fun (c: TfsCommits.Value) -> c.CommitId)
      >> commitDetailsCache getCommitDetails
      >> TfsCommitDetails.Parse
      >> getCommitInfoFromDetailedJsonObj
    commits.[true] |> List.map mapFn

  [notTruncated; truncated] |> List.concat
