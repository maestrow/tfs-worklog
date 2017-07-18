module TfsUrlTemplates

open System
open FSharp.Reflection

open Utils.Reflection
open Utils.WebUtils

module private Internals = 
  let toString (value: obj) = 
    match value with
    | :? DateTime as dt -> dt.ToString("yyyy-MM-dd")
    | _ -> value.ToString()

  let getBaseUrl schema host collection project =
    sprintf "%s://%s/%s/%s/" schema host collection project

  let baseUrl (res: string) = 
    fun a b c d -> 
      (getBaseUrl a b c d) + res

  let baseUrl1 fn = 
    fun a b c d e -> 
      (getBaseUrl a b c d) + (fn e)

  let baseUrl2 fn = 
    fun a b c d e f -> 
      (getBaseUrl a b c d) + (fn e f)

  let baseUrl3 fn = 
    fun a b c d e f g -> 
      (getBaseUrl a b c d) + (fn e f g)


open Internals

type CommitsParams = {
  committer: string option
  itemPath: string option
  fromDate: DateTime option
  toDate: DateTime option
} with static member Default = makeEmptyRecord typeof<CommitsParams>

module Tpl = 

  let repositories = baseUrl "_apis/git/repositories"

  let commitDetails = baseUrl2 <| fun repoId commitId ->
    sprintf 
      "_apis/git/repositories/%s/commits/%s" 
      repoId
      commitId

  let commits = baseUrl2 <| fun repoId (parameters: CommitsParams) ->
    sprintf 
      "_apis/git/repositories/%s/commits?%s" 
      repoId
      (toUrl toString parameters)

module TplUi = 
  let task = baseUrl1 <| fun taskId ->
    sprintf "_workitems/edit/%i" taskId

  let commit = baseUrl2 <| fun repoId commitId ->
    sprintf "_git/%s/commit/%s" repoId commitId