#r "../packages/FSharp.Data/lib/net40/FSharp.Data.dll"
#r "../packages/FSharp.Data/lib/net40/FSharp.Data.DesignTime.dll"
#r "System.Xml.Linq.dll"
#load "../WorkLog/Domain/Tfs.fs"

open FSharp.Data
open Domain.Tfs

let commits = TfsCommits.GetSample()

commits.Value
|> List.ofArray
|> List.iter (fun c -> printf "%A" c)