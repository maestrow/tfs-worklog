module Utils.StringCache

open System
open System.IO
open System.Reflection

let private cache dir (f : string -> string) (key: string) :  string =
  let path = Path.Combine(dir, key)
  if (File.Exists (path)) then 
    File.ReadAllText path
  else
    let result = f key
    File.WriteAllText (path, result)
    result

let initCache dirName = 
  let path = Path.Combine (Path.GetDirectoryName (Assembly.GetExecutingAssembly()).Location, dirName)
  if not (Directory.Exists path) then
    Directory.CreateDirectory path |> ignore
  else ()
  cache path