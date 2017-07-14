module Utils.SuaveHelpers

open Suave.Utils

let getParam (q: NameOptionValueList) parser name defaultValue = 
  match q ^^ name with
  | Choice1Of2 v -> parser v
  | Choice2Of2 _ -> defaultValue

