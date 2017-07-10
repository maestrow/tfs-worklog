open FSharp.Core
open System
open System.IO
open System.Text.RegularExpressions

let path = Path.Combine (__SOURCE_DIRECTORY__, "../WorkLog/TestData/sampleText.txt")

let text = File.ReadAllText path

Regex.Split(text, @"(?<=\.\.\.|(?!\.\.)[.!?])")
|> Seq.iter (fun s -> printfn "%s" s)

Reflection.Assembly.GetExecutingAssembly().Location