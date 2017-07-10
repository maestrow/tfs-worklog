module Domain.UserInput

open System
open System.Text.RegularExpressions
open Suave.Utils
open Extensions

module private Internals =
  
  let parseDate str =
    let rx = Regex ("(?<year>\d\d\d\d)-(?<month>\d\d)-(?<day>\d\d)")
    let m = rx.Match(str)
    let ymd = ["year"; "month"; "day"] |> List.map (fun groupName -> m.Groups.Item(groupName).Value |> int)
    DateTime (ymd.[0], ymd.[1], ymd.[2])
  
  let getParam (q: (string * string option) list) parser name defaultValue = 
    let value = 
      match q ^^ name with
      | Choice1Of2 v -> parser v
      | Choice2Of2 _ -> defaultValue
    value

open Internals

type UserInput = { dateFrom: DateTime; dateTo: DateTime } with
  
  member this.ToString = 
    let format = "dd.mm.yyyy"
    sprintf "С %s по %s." (this.dateFrom.ToString format) (this.dateTo.ToString format)
  
  static member FromParameters (q: (string * string option) list) = 
    let p = getParam q
    let (dateFromDefault,dateToDefault) = DateTime.Now.GetMonthBoundaries ()
    { 
      dateFrom = p parseDate "dateFrom" dateFromDefault
      dateTo = p parseDate "dateTo" dateToDefault
    }
