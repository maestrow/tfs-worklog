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

let r1 = 
  [
    "collection", "collectionValue"
    "project", "projectValue"
    "repoId", "repoIdValue"
  ]

let c1 = 
  [
    "committer", "John Doe"
    "itemPath", null
    "fromDate", "2017-06-01"
    "toDate", "2017-06-30"
  ]

let inline (~~) l = l |> List.map (fun (k, v) -> k, Some v) |> dict
let d = D.deserializers |> dict |> D.fromString

let result = D.createObjFormQuery d ~~c1 typeof<CommitsParams>

printfn "%A" result