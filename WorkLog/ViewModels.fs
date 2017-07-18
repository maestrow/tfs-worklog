module Web.ViewModels

open System
open System.Runtime.Serialization

open TfsUrlTemplates

type Activities = Activity list

and Activity = 
  | Commit of CommitInfo


and CommitInfo = { 
  id: string
  date: DateTime 
  issueId: int
  message: string
  
  schema: string
  host: string
  collection: string
  project: string
  repoId: string
} with
  member private v.baseUrl fn = fn v.schema v.host v.collection v.project 
  member v.issueUrl = v.baseUrl TplUi.task v.issueId
  member v.commitUrl = v.baseUrl TplUi.commit v.repoId v.id


type [<DataContract>] CommitInfoXml = { 
  [<field: DataMember(Name="id")>]
  id: string
  [<field: DataMember(Name="date")>]
  date: string 
  [<field: DataMember(Name="issueId")>]
  issueId: int
  [<field: DataMember(Name="issueUrl")>]
  issueUrl: string
  [<field: DataMember(Name="commitUrl")>]
  commitUrl: string 
  [<field: DataMember(Name="message")>]
  message: string
}

let mapCommitInfo (src: CommitInfo) : CommitInfoXml = 
  {
    CommitInfoXml.id = src.id
    date = src.date.ToString("dd.MM.yyyy HH:mm:ss")
    issueId = src.issueId
    issueUrl = src.issueUrl
    commitUrl = src.commitUrl
    message = src.message
  }