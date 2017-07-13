module Utils.Reflection

open System
open FSharp.Reflection

let makeEmptyRecord (recordType: Type) = 
  let paramValues = recordType |> FSharpType.GetRecordFields |> Array.map (fun _ -> None |> box)
  FSharpValue.MakeRecord (recordType, paramValues)
