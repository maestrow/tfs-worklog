module Tests

open Expecto
open Swensen.Unquote

[<Tests>]
let tests = testList "test group" [
    testCase "yes" <| fun _ ->
        test <@ ([3; 2; 1; 0] |> List.map ((+) 1)) = [4; 3; 2; 1] @>
]


[<EntryPoint>]
let main argv =
    runTestsInAssembly defaultConfig argv
