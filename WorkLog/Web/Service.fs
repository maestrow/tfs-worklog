module Web.Service

open System
open System.Text.RegularExpressions
open Suave.Utils
open Utils.SuaveHelpers

module Tpl = Logic.TfsUrlTemplates

module private Internals =
  
  let parseDate str =
    let rx = Regex ("(?<year>\d\d\d\d)-(?<month>\d\d)-(?<day>\d\d)")
    let m = rx.Match(str)
    let ymd = ["year"; "month"; "day"] |> List.map (fun groupName -> m.Groups.Item(groupName).Value |> int)
    DateTime (ymd.[0], ymd.[1], ymd.[2])
  
open Internals










let FromParameters (q: (string * string option) list) = 
    let p = getParam q
    let (dateFromDefault,dateToDefault) = DateTime.Now.GetMonthBoundaries ()
    { 
      dateFrom = p parseDate "dateFrom" dateFromDefault
      dateTo = p parseDate "dateTo" dateToDefault
    }
