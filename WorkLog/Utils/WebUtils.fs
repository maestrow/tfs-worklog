module Utils.WebUtils

open System
open FSharp.Reflection

module private Internals = 
  let getValueFromOption (optionValue: obj) = 
    let t = optionValue.GetType()
    let valueProp = t.GetProperty("Value")
    valueProp.GetValue(optionValue)

open Internals

// Convert record with optional fields to URL like: name1=value2&name2=value2&...
let toUrl (toStringFn: obj -> string) (record: obj) = 
  let fieldNames = 
    FSharpType.GetRecordFields (record.GetType())
    |> Array.map (fun pi -> pi.Name)
  let fieldValues = FSharpValue.GetRecordFields record 
  Seq.map2 (fun n v -> n, v) fieldNames fieldValues
  |> Seq.filter (fun (_, v) -> v <> null)
  |> Seq.map (fun (n, v) -> n, v |> getValueFromOption |> toStringFn)
  |> Seq.map (fun (n, v) -> sprintf "%s=%s" n v)
  |> String.concat "&"