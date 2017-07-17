module Web.ViewModels

open System
open System.Runtime.Serialization

type Activities = Activity list

and Activity = 
  | Commit of CommitInfo


and [<DataContract>] CommitInfo = { 
  [<field: DataMember(Name="id") >]
  id: string
  [<field: DataMember(Name="date") >]
  date: DateTime 
  [<field: DataMember(Name="issueUrl") >]
  issueUrl: string
  [<field: DataMember(Name="issueId") >]
  issueId: int
  [<field: DataMember(Name="message") >]
  message: string
  [<field: DataMember(Name="url") >]
  url: string 
} with
static member GetUrl (id: string) = 
  sprintf "http://hohoho.ru/%s" id


