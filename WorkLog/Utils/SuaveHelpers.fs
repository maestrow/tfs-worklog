module Utils.SuaveHelpers

open System
open System.Collections.Generic
open FSharp.Reflection
open Suave.Utils

let getParam (q: NameOptionValueList) parser name defaultValue = 
  match q ^^ name with
  | Choice1Of2 v -> parser v
  | Choice2Of2 _ -> defaultValue

let deserializers = [
  typeof<string>, fun i -> box i
  typeof<DateTime>, fun i -> box (DateTime.Parse i)
  typeof<int>, fun i -> box (int i)
]

let fromString (d: IDictionary<Type, string -> obj>) (resultType: Type) (str: string) : Choice<obj, string> = 
  if d.ContainsKey(resultType) then 
    // ToDo: option type
    try
      str |> d.[resultType] |> Choice1Of2
    with
      | _ as ex -> Choice2Of2 (sprintf "Error occured when deserializing value \"%s\" of type %A. Exception message: %s" str resultType ex.Message)
  else
    Choice2Of2 (sprintf "There is no registered deserializer for type %A" resultType)

let private createRecordFormQuery (deserializer: Type -> string -> Choice<obj, string>) (q: NameOptionValueList) (t: Type) : Choice<obj, string> = 
  let qDict = q |> dict
  let fieldsResult = 
    FSharpType.GetRecordFields t
    |> List.ofArray
    |> List.filter (fun i -> qDict.ContainsKey(i.Name) && qDict.[i.Name].IsSome)
    |> List.map (fun i -> i.Name, deserializer i.PropertyType qDict.[i.Name].Value)
  let error = 
    fieldsResult
    |> List.filter (
      snd
      >> function Choice2Of2 _ -> true | _ -> false)
    |> List.map (fun (_, Choice2Of2 v) -> v)
    |> String.concat "\n"
  if error <> String.Empty then
    error |> Choice2Of2
  else
    let values = 
      fieldsResult
      |> List.map (fun (_, Choice1Of2 v) -> v)
      |> Array.ofList
    FSharpValue.MakeRecord (t, values) |> Choice1Of2

let createObjFormQuery (deserializer: Type -> string -> Choice<obj, string>) (q: NameOptionValueList) (t: Type) : Choice<obj, string> = 
  if FSharpType.IsRecord t then
    createRecordFormQuery deserializer q t
  else
    Choice2Of2 "I am able to create only records so far"

