#load "../paket-files/haf/YoLo/YoLo.fs"
#load "../WorkLog/Utils/Deserializer.fs"

open System
module D = Utils.Deserializer

type RepoParams = {
  collection: string
  project: string
  repoId: string 
}

type CommitsParams = {
  committer: string option
  itemPath: string option
  fromDate: DateTime option
  toDate: DateTime option
} 

let q1 = 
  [
    "collection", "collectionValue"
    "project", "Value"
    "repoId", "Value"
  ]
  |> List.map (fun (k, v) -> k, Some v)
  |> dict

let d = D.deserializers |> dict |> D.fromString
let result = D.createObjFormQuery d q1 typeof<RepoParams>

printfn "%A" result