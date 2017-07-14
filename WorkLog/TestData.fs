module TestData

open System
open System.IO
open System.Text.RegularExpressions
open Web.ViewModels
open Extensions
open TestDataGenerators

module Generators = 

  let createCommitInfo id date issueId message = 
    { 
      id = id
      date = date
      issueId = issueId
      message = message
      url = CommitInfo.GetUrl id
    }

  let createMessageGen path (n: Gen<int>) : Gen<string> =
    let text = File.ReadAllText path
    let sentenсes = Regex.Split(text, @"(?<=\.\.\.|(?!\.\.)[.!?])")
    let total = sentenсes.Length
    let random = Random()
    // n - количество предложений текста, которое следует венруть
    fun () ->
      let count = n ()
      let selected = random.Next(1, total - count)
      sentenсes.[selected..selected + count - 1]
      |> String.concat ""

  let issueId : Gen<int> = 
    let digitsCountGen = Gen.intRange 4 5
    let digitGen = Gen.intRange 0 9
    fun () -> 
      [1 .. (digitsCountGen())]
      |> List.map ((fun _ -> ()) >> digitGen >> ((+) 48) >> char)
      |> Array.ofList
      |> String
      |> int

  let dates = 
    let (dateFrom,dateTo) = DateTime.Now.GetMonthBoundaries ()
    let rSort = Random ()
    let activeDays = Gen.intRange 18 22
    let dayCommitsCountGen = Gen.intRange 1 10
    let rDayOffet = Random()
    [dateFrom.Day .. dateTo.Day]
    |> Seq.map (fun day -> dateFrom.AddDays (float (day-1)))
    |> Seq.sortBy (fun _ -> rSort.NextDouble ())
    |> Seq.take (activeDays())
    |> Seq.map (fun d -> [1..dayCommitsCountGen ()] |> Seq.map (fun _ -> d.AddDays(rDayOffet.NextDouble())))
    |> Seq.concat

  let message : Gen<string> = 
    createMessageGen 
      (Path.Combine (__SOURCE_DIRECTORY__, "TestData/sampleText.txt"))
      (Gen.intRange 1 3)

  let generateCommitInfo d =
    createCommitInfo (Gen.guid() |> string) d (issueId()) (message ())


open Generators

let generateActivities () : Activities = 
  dates
  |> List.ofSeq
  |> List.map (generateCommitInfo >> Commit)