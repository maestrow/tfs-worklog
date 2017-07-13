module Web.ViewModels

open System

type Activities = Activity list

and Activity = 
  | Commit of CommitInfo

and CommitInfo = { 
  id: string
  date: DateTime 
  issueId: int
  message: string
  url: string 
} with
static member GetUrl (id: string) = 
  sprintf "http://hohoho.ru/%s" id


