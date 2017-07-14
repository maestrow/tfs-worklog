module Utils.Deserializer

open System
open System.Collections.Generic
open FSharp.Reflection

type QueryParams = IDictionary<string, string option>
type Deserializer = Type -> string -> Choice<obj, string>


module private Internals = 
  let applyDeserializer (d: string -> obj) (str: string) : Choice<obj, string> = 
    try
      str |> d |> Choice1Of2
    with
      | _ as ex -> Choice2Of2 ex.Message

  let isOption (t: Type) = t.IsGenericType && t.GetGenericTypeDefinition() = typedefof<option<_>>

  let get1Of2 = function
    | Choice1Of2 v -> v
    | Choice2Of2 _ -> failwith "Wrong Choice"
  
  let get2Of2 = function
    | Choice1Of2 _ -> failwith "Wrong Choice"
    | Choice2Of2 v -> v

  let with1Of2 = function Choice1Of2 _ -> true | _ -> false

  let with2Of2 = function Choice2Of2 _ -> true | _ -> false

open Internals

let deserializers = [
  typeof<string>, fun i -> box i
  typeof<DateTime>, fun i -> box (DateTime.Parse i)
  typeof<int>, fun i -> box (int i)
]


let fromString (d: IDictionary<Type, string -> obj>) (resultType: Type) (str: string) : Choice<obj, string> = 
  let isOpt = isOption resultType
  let t = if isOpt then resultType.GetGenericArguments().[0] else resultType

  if d.ContainsKey(t) then 
    match applyDeserializer d.[t] str with
    | Choice1Of2 o -> Choice1Of2 (if isOpt then o |> Some |> box else o)
    | Choice2Of2 msg -> Choice2Of2 (sprintf "Error occured when deserializing value \"%s\" of type %A. Exception message: %s" str t msg)
  else
    Choice2Of2 (sprintf "There is no registered deserializer for type %A" resultType)

let private createRecordFormQuery (deserializer: Deserializer) (q: QueryParams) (t: Type) : Choice<obj, string> = 
  let fieldsResult = 
    FSharpType.GetRecordFields t
    |> List.ofArray
    |> List.filter (fun i -> q.ContainsKey(i.Name) && q.[i.Name].IsSome)
    |> List.map (fun i -> i.Name, deserializer i.PropertyType q.[i.Name].Value)
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

let createObjFormQuery (d: Deserializer) (q: QueryParams) (t: Type) : Choice<obj, string> = 
  if FSharpType.IsRecord t then
    createRecordFormQuery d q t
  else
    Choice2Of2 "I am able to create only records so far"

let getParamsFromQuery (d: Deserializer) (q: QueryParams) (types: Type list) : Choice<obj list, string list> = 
  let creator = createObjFormQuery d q
  let result = types |> List.map (fun t -> creator t)
  let errors = result |> List.filter with2Of2 |> List.map get2Of2
  if errors.Length > 0 then 
    Choice2Of2 errors 
  else 
    result
    |> List.filter with1Of2
    |> List.map get1Of2
    |> Choice1Of2 
  

