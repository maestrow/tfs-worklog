module Logic.Commits

open System
open System.IO
open System.Net
open System.Reflection
open System.Text.RegularExpressions

open Suave
open FSharp.Data

open Domain.UserInput
open Domain.Settings
open Domain.Tfs
open ViewModels

module private Implementation = 

  let settings =
    let root = Path.GetDirectoryName (Assembly.GetExecutingAssembly()).Location
    let xml = Path.Combine (root, "settings.xml") |> File.ReadAllText
    Settings.Parse xml

  let credentials = 
      let c = settings.NetworkCredential
      NetworkCredential(c.Login, c.Password, c.Domain)

  let addCredentials (req: HttpWebRequest) = 
      req.Credentials <- credentials
      req

  let request url = Http.RequestString (url, customizeHttpRequest = addCredentials)

  let reqCommitDetails commitId = 
    let url = sprintf "https://tfsdsr.norbit.ru/TradingSystems/_apis/git/repositories/50c80d8d-4a80-415b-90a8-5659a6693aa2/commits/%s" commitId
    request url

  let reqCommits (userInput: UserInput) = 
    let format = "yyyy-MM-dd"
    let fromDate = userInput.dateFrom.ToString format
    let toDate = userInput.dateTo.ToString format
    let url = sprintf "https://tfsdsr.norbit.ru/TradingSystems/_apis/git/repositories/50c80d8d-4a80-415b-90a8-5659a6693aa2/commits?committer=shmelev&fromDate=%s&toDate=%s" fromDate toDate
    request url

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

let getCommits (userInput: UserInput) =
  let commits = 
    reqCommits userInput
    |> TfsCommits.Parse
    |> (fun root -> root.Value)
    |> List.ofArray
    |> List.groupBy (fun c -> c.CommentTruncated.IsSome && c.CommentTruncated.Value)
    |> dict

  let list1 = 
    commits.[false]
    |> List.map getCommitInfoFromShortJsonObj
  
  let list2 = 
    commits.[true]
    |> List.map (fun c -> reqCommitDetails c.CommitId |> TfsCommitDetails.Parse)
    |> List.map getCommitInfoFromDetailedJsonObj

  [list1; list2] |> List.concat
