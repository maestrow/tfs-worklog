open System

let a : Choice<int, int> = Choice2Of2 1

let f = function
        | Choice1Of2 e -> printfn "1 %A" e
        | Choice2Of2 e -> printfn "2 %A" e

f a
