module DeserializerTests

open Expecto
open Swensen.Unquote

open Utils.SuaveHelpers

[<Tests>]
let tests = testList "DeserializerTests" [
    testCase "yes" <| fun _ ->
        test <@ ([3; 2; 1; 0] |> List.map ((+) 1)) = [4; 3; 2; 1] @>
]