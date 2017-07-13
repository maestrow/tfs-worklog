module Logic.TfsUrlTemplates

open System
open FSharp.Reflection

open Utils.Reflection
open Utils.WebUtils

module private Internals = 
  let toString (value: obj) = 
    match value with
    | :? DateTime as dt -> dt.ToString("yyyy-MM-dd")
    | _ -> value.ToString()

open Internals

type CommitsParams = {
  committer: string option
  itemPath: string option
  fromDate: DateTime option
  toDate: DateTime option
} with static member Default = makeEmptyRecord typeof<CommitsParams>

module Tpl = 

  let repositories schema host collection project = 
    sprintf
      "%s://%s/%s/%s/_apis/git/repositories"
      schema
      host
      collection
      project

  let commitDetails schema host collection project repoId commitId = 
    sprintf 
      "%s://%s/%s/%s/_apis/git/repositories/%s/commits/%s" 
      schema
      host
      collection
      project
      repoId
      commitId

  let commits schema host collection project repoId (parameters: CommitsParams) = 
    sprintf 
      "%s://%s/%s/%s/_apis/git/repositories/%s/commits?%s" 
      schema
      host
      collection
      project
      repoId
      (toUrl toString parameters)
